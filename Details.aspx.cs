using Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Final
{
    public partial class Details : System.Web.UI.Page
    {
        List<Adventurer> adventurers = new List<Adventurer>();
        List<Item> items = Helper.GetAvailableItems();

        int index = -1;

        protected void Page_Load(object sender, EventArgs e)
        {
            Get_Index();
            Get_Adventurers();
            Reset_ErrorMessages();

            if (!IsPostBack) { Populate_Items(); }
            Display_AdventurerDetails();
        }

        protected void Get_Index()
        {
            if (Request.QueryString["id"] == null)
            {
                Response.Redirect("Default.aspx");
            }

            index = int.Parse(Request.QueryString["id"]);
        }

        protected void Get_Adventurers()
        {
            if (Session["adventurers"] != null)
            {
                adventurers = (List<Adventurer>)Session["adventurers"];
            }
        }

        protected void Reset_ErrorMessages()
        {
            lblErrorMessages.Visible = false;
            lblErrorMessages.Text = string.Empty;
        }

        protected void Populate_Items()
        {
            int itemIndex = 0;
            foreach (Item item in items)
            {
                ListItem listItem = new ListItem($"{item.Name} ({item.StrengthRequirement}/{item.DexterityRequirement}/{item.ManaRequirement})", itemIndex.ToString());

                if (adventurers.Count > 0)
                {
                    if (adventurers[index].Item_Equiped(item))
                    {
                        listItem.Selected = true;
                    }
                }

                cblItems.Items.Add(listItem);

                itemIndex++;
            }
        }

        protected void Display_AdventurerDetails()
        {
            if (index >= 0 && index < adventurers.Count)
            {
                Adventurer adventurer = adventurers[index];
                txtName.InnerText = adventurer.Name;
                txtType.InnerText = adventurer.Type;
                txtPhrase.InnerText = adventurer.Greeting();

                lblStrength.Text = adventurer.Strength.ToString();
                lblDexterity.Text = adventurer.Dexterity.ToString();
                lblVitality.Text = adventurer.Vitality.ToString();
                lblMana.Text = adventurer.Mana.ToString();
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }

        protected void btnEquipItems_Click(object sender, EventArgs e)
        {
           
            if (index >= 0 && index < adventurers.Count)
            {
                Adventurer adventurer = adventurers[index];

                adventurer.EquippedItems.Clear();

                List<Item> unequippedItems = new List<Item>();

                foreach (ListItem itemListItem in cblItems.Items)
                {
                    int itemIndex = int.Parse(itemListItem.Value);
                    Item item = items[itemIndex];

                    if (itemListItem.Selected)
                    {
                        try
                        {
                            adventurer.Equip_Item(item);
                        }
                        catch (Exception ex)
                        {
                            unequippedItems.Add(item);
                        }
                    }
                }

                if (unequippedItems.Count > 0)
                {
                    lblErrorMessages.Visible = true;
                    
                    foreach (Item unequippedItem in unequippedItems)
                    {
                        lblErrorMessages.Text += $"{unequippedItem.Name} cannot be equipped.<br>";
                    }
                


                foreach (ListItem itemListItem in cblItems.Items)
                    {
                        int itemIndex = int.Parse(itemListItem.Value);
                        Item item = items[itemIndex];

                        if (unequippedItems.Contains(item))
                        {
                            itemListItem.Selected = false;
                        }
                    }
                }
                else
                {
                    
                    Session["adventurers"] = adventurers;
                    Response.Redirect($"Details.aspx?id={index}");
                }
            }
        }
    }
}