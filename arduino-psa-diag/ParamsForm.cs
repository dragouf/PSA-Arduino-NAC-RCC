using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Newtonsoft.Json.Linq;

namespace arduino_psa_diag
{
    public class ParamsForm : Form
    {
        private Hashtable _UserZoneValueHash = new Hashtable();

        private Hashtable NacZoneValueHash;

        private string LanguageCode = "fr";

        private IContainer components = null;

        private SplitContainer SplitContainer1;

        private Button ButtonSave;

        private Button ButtonCancel;

        private TextBox TextBoxVin;

        private Label LabelNacSerial;

        private Label LabelVin;

        private TextBox TextBoxSN;

        private TextBox TextBoxBtName;

        private Label LabelBtName;

        private Label LabelZoneSummary2;

        private Label LabelZoneSummary1;

        private TabPage TabPage1;
        private TabControl TabControl1;

        public static void ReloadBuffer(Control P_0)
        {
            PropertyInfo property = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            property.SetValue(P_0, true, null);
        }

        public Hashtable UserKeyValueHash()
        {
            return this._UserZoneValueHash;
        }

        public ParamsForm()
        {
            InitializeComponent();
            if (!CultureInfo.InstalledUICulture.NativeName.Contains("fran"))
            {
                this.LanguageCode = "en";
            }
        }

        private void OnFormLoad(object sender, EventArgs args)
        {
        }

        public void InitForm(ref Hashtable nacZoneValueHash, ref JObject jsonObject)
        {
            this.NacZoneValueHash = nacZoneValueHash;

            checked
            {
                // F190	VIN
                // F18C	Serial number
                // 0106	Calibration_Fct_BT
                if (nacZoneValueHash.ContainsKey("F190") && nacZoneValueHash.ContainsKey("F18C") && nacZoneValueHash.ContainsKey("0106"))
                {
                    // VIN
                    string zoneNacValue = nacZoneValueHash["F190"].ToString();

                    if (zoneNacValue != "" && zoneNacValue != "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF")
                    {
                        for (int i = 0; i < zoneNacValue.Length; i += 2)
                        {
                            this.TextBoxVin.Text += Convert.ToChar(byte.Parse(zoneNacValue.Substring(i, 2), NumberStyles.HexNumber));
                        }
                        this.TextBoxVin.Tag = zoneNacValue;
                    }
                    else
                    {
                        this.TextBoxVin.Text = "?????????????????";
                        this.TextBoxVin.Tag = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                    }

                    // Bluetooth
                    Label label = this.LabelBtName;
                    bool visible = (this.TextBoxBtName.Visible = true);
                    label.Visible = visible;
                    this.TextBoxBtName.Tag = nacZoneValueHash["0106"].ToString();
                    zoneNacValue = nacZoneValueHash["0106"].ToString().Replace("00", "");

                    for (int i = 0; i < zoneNacValue.Length; i += 2)
                    {
                        this.TextBoxBtName.Text += Convert.ToChar(byte.Parse(zoneNacValue.Substring(i, 2), NumberStyles.HexNumber));
                    }

                    // SN
                    zoneNacValue = nacZoneValueHash["F18C"].ToString();
                    if (zoneNacValue != null)
                    {
                        for (int i = 0; i < zoneNacValue.Length; i += 2)
                        {
                            this.TextBoxSN.Text += Convert.ToChar(byte.Parse(zoneNacValue.Substring(i, 2), NumberStyles.HexNumber));
                        }
                        this.TextBoxSN.Tag = this.TextBoxSN.Text;
                    }
                    else
                    {
                        this.TextBoxSN.Tag = "";
                    }

                    int labelXPosStart = 16;
                    int labelYPosStart = 21;

                    Regex regex = new Regex("(DZK|DZR|E5B|E88|EIO)");
                    int paramIndex = 0;

                    List<string> zonesLabels = new List<string>();

                    foreach (JProperty jsonZone in jsonObject.Root["NAC"]["zones"].Children())
                    {
                        string zoneNodeCode = jsonZone.Name;
                        if (jsonZone.Name.Contains("0106") || jsonZone.Name.Contains("F190") || jsonZone.Name.Contains("F18C") || !this.NacZoneValueHash.ContainsKey(zoneNodeCode))
                        {
                            continue;
                        }

                        var zoneName = jsonZone.Value["name"]?.ToString();
                        var zoneDescription = jsonZone.Value["description"][this.LanguageCode]?.ToString() ?? "";
                        var zoneAcronym = jsonZone.Value["acronyms"]?.ToString();

                        var zoneLabel = $"{zoneNodeCode}: {zoneDescription} [{zoneAcronym}]\n";

                        zonesLabels.Add(zoneLabel);

                        TabPage tabPage = new TabPage(zoneNodeCode)
                        {
                            AutoScroll = true,
                            BackColor = SystemColors.Control,
                            Padding = new Padding(3),
                            TabIndex = 0,
                            Name = jsonZone.Name,
                            ToolTipText = string.IsNullOrEmpty(zoneDescription) ? zoneAcronym : zoneDescription
                        };

                        int currentYPos = labelYPosStart;
                        paramIndex = 0;

                        Label labelZoneCode = new Label
                        {
                            Location = new Point(labelXPosStart, currentYPos + paramIndex * 30),
                            Name = jsonZone.Value["name"].ToString(),
                            Text = zoneNodeCode + ": " + zoneName,
                            Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold, GraphicsUnit.Point, 0),
                            Size = new Size(350, 25),
                            TabIndex = 4
                        };

                        tabPage.Controls.Add(labelZoneCode);

                        Label labelZoneDesc = new Label
                        {
                            Location = new Point(labelXPosStart + 350, currentYPos + paramIndex * 30),
                            Name = jsonZone.Value["name"].ToString() + "_desc",
                            Text = $"({zoneDescription})",
                            Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Italic, GraphicsUnit.Point, 0),
                            Size = new Size(450, 25)
                        };

                        tabPage.Controls.Add(labelZoneDesc);

                        currentYPos += 30;

                        foreach (JObject zoneParams in jsonZone.Value["params"].Children())
                        {
                            Label labelExtraName = new Label
                            {
                                Location = new Point(labelXPosStart, currentYPos + paramIndex * 30),
                                Name = zoneParams["extra_name"].ToString(),
                                Text = zoneParams["name"].ToString() + ": " + zoneParams["extra_name"].ToString(),
                                Size = new Size(310, 32),
                                TabIndex = 4
                            };

                            if (((JObject)zoneParams["detail"])[this.LanguageCode].ToString() != "")
                            {
                                labelExtraName.Text = ((JObject)zoneParams["detail"])[this.LanguageCode].ToString();
                            }

                            tabPage.Controls.Add(labelExtraName);

                            int zoneNacDataValue = 0;
                            // listbox param
                            if (zoneParams["listbox"] != null)
                            {
                                ComboBox comboBox = new ComboBox
                                {
                                    BackColor = Color.White,
                                    DropDownStyle = ComboBoxStyle.DropDownList,
                                    FormattingEnabled = true,
                                    Location = new Point(labelXPosStart + 315, currentYPos + paramIndex * 30 - 5),
                                    Name = "comboBox1",
                                    Size = new Size(400, 21),
                                    TabIndex = 8
                                };

                                tabPage.Controls.Add(comboBox);

                                if (nacZoneValueHash.ContainsKey(zoneNodeCode))
                                {
                                    zoneNacValue = nacZoneValueHash[zoneNodeCode].ToString();
                                    if (zoneNacValue != "")
                                    {
                                        int paramPos = int.Parse(zoneParams["pos"].ToString()) - 4;

                                        // 2108	Telecoding_Fct_BTEL
                                        if (zoneNacValue.Length < 20 && zoneNodeCode == "2108" && regex.Match(zoneParams["name"].ToString()).Success)
                                        {
                                            paramPos--;
                                        }

                                        zoneNacDataValue = byte.Parse(zoneNacValue.Substring(paramPos * 2, 2), NumberStyles.HexNumber);
                                        byte paramMask = byte.Parse(zoneParams["mask"].ToString(), NumberStyles.HexNumber);
                                        zoneNacDataValue &= paramMask;
                                    }
                                }

                                foreach (JObject paramListJson in zoneParams["listbox"].Children())
                                {
                                    comboBox.Items.Add(((JObject)paramListJson["text"])[this.LanguageCode].ToString());
                                    if (paramListJson["value"].ToString() == zoneNacDataValue.ToString("X2"))
                                    {
                                        comboBox.SelectedIndex = comboBox.Items.Count - 1;
                                    }
                                }

                                JObject jsonParamsClone = (JObject)zoneParams.DeepClone();
                                jsonParamsClone.Add(new JProperty("zone", zoneNodeCode));

                                if (comboBox.SelectedIndex == -1)
                                {
                                    comboBox.Items.Add("Unknown 0x" + zoneNacDataValue.ToString("X2"));
                                    comboBox.SelectedIndex = comboBox.Items.Count - 1;
                                    jsonParamsClone.Add(new JProperty("unknown", zoneNacDataValue.ToString("X2")));
                                }

                                comboBox.Tag = jsonParamsClone;
                            }
                            // textbox
                            else if (zoneParams["mask"].ToString() == "FF")
                            {
                                string zoneValueReadable = "??";
                                string paramUnit = "";

                                if (nacZoneValueHash.ContainsKey(zoneNodeCode))
                                {
                                    zoneNacValue = nacZoneValueHash[zoneNodeCode].ToString();
                                    if (zoneNacValue != "")
                                    {
                                        int paramPos = int.Parse(zoneParams["pos"].ToString()) - 4;
                                        int length = int.Parse(zoneParams["size"].ToString()) * 2;

                                        if (zoneNacValue.Length < 20 && zoneNodeCode == "2108" && regex.Match(zoneParams["name"].ToString()).Success)
                                        {
                                            paramPos--;
                                        }

                                        zoneNacDataValue = int.Parse(zoneNacValue.Substring(paramPos * 2, length), NumberStyles.HexNumber);
                                        zoneValueReadable = ((!(zoneParams["name"].ToString() == "DSZ")) ? Convert.ToInt32(zoneNacDataValue).ToString() : (3.0 + (double)(zoneNacDataValue - 86) * 0.5).ToString("#.#"));
                                        if (zoneParams["unit"] != null)
                                        {
                                            paramUnit = zoneParams["unit"].ToString();
                                        }
                                    }
                                }

                                TextBox textboxParamValue = new TextBox
                                {
                                    Location = new Point(labelXPosStart + 315, currentYPos + paramIndex * 30),
                                    Name = "valeur",
                                    Text = zoneValueReadable,
                                    Size = new Size(30, 23),
                                    TabIndex = 4
                                };

                                JObject zoneParamsClone = (JObject)zoneParams.DeepClone();
                                zoneParamsClone.Add(new JProperty("zone", zoneNodeCode));

                                textboxParamValue.Tag = zoneParamsClone;

                                tabPage.Controls.Add(textboxParamValue);

                                Label labelParamUnit = new Label
                                {
                                    Location = new Point(labelXPosStart + 355, currentYPos + paramIndex * 30),
                                    Name = "unit",
                                    Text = paramUnit,
                                    Size = new Size(300, 23),
                                    TabIndex = 4
                                };

                                tabPage.Controls.Add(labelParamUnit);
                            }
                            // textbox
                            else if (zoneParams["maskBinary"].ToString().Contains("11"))
                            {
                                zoneNacValue = nacZoneValueHash[zoneNodeCode].ToString();
                                int paramPos = int.Parse(zoneParams["pos"].ToString()) - 4;

                                if (zoneNacValue.Length < 20 && zoneNodeCode == "2108" && regex.Match(zoneParams["name"].ToString()).Success)
                                {
                                    paramPos--;
                                }

                                zoneNacDataValue = byte.Parse(zoneNacValue.Substring(paramPos * 2, 2), NumberStyles.HexNumber);
                                byte paramMask = byte.Parse(zoneParams["mask"].ToString(), NumberStyles.HexNumber);
                                zoneNacDataValue &= paramMask;
                                paramPos = zoneParams["maskBinary"].ToString().LastIndexOf('1');
                                paramPos = 8 - paramPos - 1;
                                zoneNacDataValue = (byte)(zoneNacDataValue >> paramPos);

                                TextBox textBox = new TextBox
                                {
                                    Location = new Point(labelXPosStart + 315, currentYPos + paramIndex * 30),
                                    Name = ((JObject)zoneParams["detail"])[this.LanguageCode].ToString(),
                                    Text = Convert.ToInt32(zoneNacDataValue).ToString(),
                                    Size = new Size(30, 23),
                                    TabIndex = 4
                                };

                                JObject zoneParamsClone = (JObject)zoneParams.DeepClone();
                                zoneParamsClone.Add(new JProperty("zone", zoneNodeCode));

                                textBox.Tag = zoneParamsClone;
                                tabPage.Controls.Add(textBox);
                            }
                            // checkbox
                            else
                            {
                                zoneNacValue = nacZoneValueHash[zoneNodeCode].ToString();
                                int paramPos = int.Parse(zoneParams["pos"].ToString()) - 4;

                                if (zoneNacValue.Length < 20 && zoneNodeCode == "2108" && regex.Match(zoneParams["name"].ToString()).Success)
                                {
                                    paramPos--;
                                }

                                if (paramPos * 2 < zoneNacValue.Length)
                                {
                                    JObject zoneParamsClone = (JObject)zoneParams.DeepClone();
                                    zoneParamsClone.Add(new JProperty("zone", zoneNodeCode));

                                    CheckBox checkBox = new CheckBox
                                    {
                                        Location = new Point(labelXPosStart + 315, currentYPos + paramIndex * 30 - 5),
                                        Name = zoneParams["name"].ToString(),
                                        Tag = zoneParamsClone,
                                        Text = "Disabled",
                                        Size = new Size(95, 24),
                                        TabIndex = 5,
                                        UseVisualStyleBackColor = true,
                                    };
                                    checkBox.CheckedChanged += new System.EventHandler(OnCheckboxChanged);


                                    tabPage.Controls.Add(checkBox);

                                    if (nacZoneValueHash.ContainsKey(zoneNodeCode) && zoneNacValue != "")
                                    {
                                        zoneNacDataValue = byte.Parse(zoneNacValue.Substring(paramPos * 2, 2), NumberStyles.HexNumber);
                                        byte zoneMask = byte.Parse(zoneParams["mask"].ToString(), NumberStyles.HexNumber);
                                        if ((zoneNacDataValue & zoneMask) == zoneMask)
                                        {
                                            checkBox.Checked = true;
                                        }
                                    }

                                    Label labelParamDescription = new Label
                                    {
                                        Location = new Point(labelXPosStart + 410, currentYPos + paramIndex * 30),
                                        Name = ((JObject)zoneParams["detail"])[this.LanguageCode].ToString(),
                                        Text = "",
                                        Size = new Size(400, 23),
                                        TabIndex = 4
                                    };

                                    tabPage.Controls.Add(labelParamDescription);
                                }
                                // Unsupported by unit / not editable
                                else
                                {
                                    Label labelParamDescription = new Label
                                    {
                                        Location = new Point(labelXPosStart + 410, currentYPos + paramIndex * 30),
                                        Name = ((JObject)zoneParams["detail"])[this.LanguageCode].ToString(),
                                        Text = "Unsupported by your unit",
                                        Size = new Size(400, 23),
                                        TabIndex = 4
                                    };

                                    tabPage.Controls.Add(labelParamDescription);
                                }
                            }

                            Label labelZoneHexValue = new Label
                            {
                                Location = new Point(labelXPosStart + 815, currentYPos + paramIndex * 30),
                                Name = "valhexa",
                                Text = zoneNacDataValue.ToString("X2"),
                                Size = new Size(50, 32),
                                TabIndex = 4
                            };

                            tabPage.Controls.Add(labelZoneHexValue);
                            paramIndex++;
                        }
                        this.TabControl1.TabPages.Add(tabPage);
                    }

                    // zone summary
                    for (int index = 0; index < zonesLabels.Count; index++)
                    {
                        if (index < unchecked(zonesLabels.Count / 2))
                        {
                            LabelZoneSummary1.Text += zonesLabels[index].ToString();
                        }
                        else
                        {
                            LabelZoneSummary2.Text += zonesLabels[index].ToString();
                        }
                    }
                }
                else
                {
                    MessageBox.Show(this, "Missing zones, please make a new reading - Disconnect PSA Diag interface if plugged", "Error");
                }

                // Reload all buffers
                foreach (Control control2 in this.SplitContainer1.Panel1.Controls)
                {
                    ReloadBuffer(control2);
                }

                foreach (Control control3 in this.SplitContainer1.Panel2.Controls)
                {
                    ReloadBuffer(control3);
                }

                ReloadBuffer(this.SplitContainer1);
            }
        }

        private void OnCheckboxChanged(object sender, EventArgs args)
        {
            if (((CheckBox)sender).Checked)
            {
                ((CheckBox)sender).Text = "Enabled";
            }
            else
            {
                ((CheckBox)sender).Text = "Disabled";
            }
        }

        private void OnButtonSaveClick(object sender, EventArgs args)
        {
            Regex regex = new Regex("^[0-9A-Z?]*$");

            if (this.TextBoxVin.Text.Length != 17 || !regex.Match(this.TextBoxVin.Text).Success)
            {
                MessageBox.Show(this, "VIN incorrect !", "Error");
                return;
            }

            regex = new Regex("^[0-9A-Za-z\\-_]*$");

            if (this.TextBoxBtName.Visible && (this.TextBoxBtName.Text.Length > 30 || this.TextBoxBtName.Text.Length == 0 || !regex.Match(this.TextBoxBtName.Text).Success))
            {
                MessageBox.Show(this, "Bluetooth Name incorrect !", "Error");
                return;
            }

            regex = new Regex("(DZK|DZR|E5B|E88|EIO)");
            checked
            {
                // GET textboxes value
                foreach (TabPage tabPage in this.TabControl1.TabPages)
                {
                    foreach (Control tabControl in tabPage.Controls)
                    {
                        if (!tabControl.GetType().ToString().Contains("TextBox") || tabControl.Tag.GetType().ToString().Contains("String"))
                        {
                            continue;
                        }

                        if (((TextBox)tabControl).Text.Contains("?"))
                        {
                            ((TextBox)tabControl).Text = "0";
                        }

                        JObject jObject = (JObject)((TextBox)tabControl).Tag;
                        string zoneCode = jObject["zone"].ToString();
                        string zoneDescription = jObject["name"].ToString() + ": " + jObject["extra_name"].ToString();

                        if (((JObject)jObject["detail"])[this.LanguageCode].ToString() != "")
                        {
                            zoneDescription = ((JObject)jObject["detail"])[this.LanguageCode].ToString();
                        }

                        int zoneValue = 0;
                        byte paramMask = byte.Parse(jObject["mask"].ToString(), NumberStyles.HexNumber);
                        int paramMaskBinaryLsbIndex = jObject["maskBinary"].ToString().LastIndexOf('1');
                        paramMaskBinaryLsbIndex = 8 - paramMaskBinaryLsbIndex - 1;
                        byte maxDigit = (byte)(paramMask >> paramMaskBinaryLsbIndex);
                        int paramSize = int.Parse(jObject["size"].ToString());

                        try
                        {
                            if (jObject["name"].ToString() == "DSZ")
                            {
                                int value = (int)((double.Parse(((TextBox)tabControl).Text.Replace(".", ",")) - 3.0) * 2.0);
                                zoneValue = Convert.ToByte(value);
                                zoneValue += 86;
                            }
                            else
                            {
                                int value2 = int.Parse(((TextBox)tabControl).Text);
                                zoneValue = Convert.ToInt16(value2);
                            }

                            double paramMaxValue = Math.Pow((int)maxDigit, paramSize);

                            unchecked
                            {
                                if ((double)zoneValue > paramMaxValue)
                                {
                                    MessageBox.Show(this, "Value greater than maximum value " + Convert.ToInt32(paramMaxValue) + " in " + zoneCode + ": " + zoneDescription, "Error");
                                    return;
                                }
                            }
                        }
                        catch
                        {
                            MessageBox.Show(this, "Incorrect value in " + zoneCode + ": " + zoneDescription, "Error");
                            return;
                        }
                    }
                }

                string paramsEditedList = "Parameters's zones are modified:\r\n";
                var clonedDefaultHashtable = (Hashtable)this.NacZoneValueHash.Clone();
                string zoneValueHex = "";
                char[] zoneValueChars = this.TextBoxVin.Text.ToCharArray();

                for (int index = 0; index < zoneValueChars.Length; index++)
                {
                    zoneValueHex += (zoneValueChars[index] != '?') ? Convert.ToByte(zoneValueChars[index]).ToString("x2") : "FF";
                }

                clonedDefaultHashtable["F190"] = zoneValueHex.ToUpper();

                if (this.TextBoxVin.Tag.ToString() != clonedDefaultHashtable["F190"].ToString())
                {
                    paramsEditedList += "F190: VIN\r\n";
                }

                if (this.TextBoxBtName.Visible)
                {
                    zoneValueHex = "";
                    zoneValueChars = this.TextBoxBtName.Text.ToCharArray();

                    for (int i = 0; i < zoneValueChars.Length; i++)
                    {
                        zoneValueHex += Convert.ToByte(zoneValueChars[i]).ToString("x2");
                    }

                    int hexValuePadLeft = 60 - zoneValueHex.Length;
                    for (int index = 0; index < hexValuePadLeft; index++)
                    {
                        zoneValueHex += "0";
                    }

                    clonedDefaultHashtable["0106"] = zoneValueHex.ToUpper();

                    if (this.TextBoxBtName.Tag.ToString() != clonedDefaultHashtable["0106"].ToString())
                    {
                        paramsEditedList += "0106: Bluetooth Name\r\n";
                    }
                }

                // GET Checkboxes/Combos value
                foreach (TabPage tabPage in this.TabControl1.TabPages)
                {
                    foreach (Control tabControl in tabPage.Controls)
                    {
                        JObject jObject;
                        byte paramMask;
                        string zoneCode;
                        int paramPos;
                        if (tabControl.GetType().ToString().Contains("CheckBox"))
                        {
                            jObject = (JObject)((CheckBox)tabControl).Tag;
                            string zoneDescription = jObject["name"].ToString() + ": " + jObject["extra_name"].ToString();
                            zoneCode = jObject["zone"].ToString();
                            paramMask = byte.Parse(jObject["mask"].ToString(), NumberStyles.HexNumber);
                            paramPos = int.Parse(jObject["pos"].ToString()) - 4;

                            if (clonedDefaultHashtable[zoneCode].ToString().Length < 20 && zoneCode == "2108" && regex.Match(zoneDescription).Success)
                            {
                                paramPos--;
                            }

                            if (clonedDefaultHashtable.ContainsKey(zoneCode))
                            {
                                byte zoneDataValueUser = 0;
                                try
                                {
                                    zoneDataValueUser = byte.Parse(clonedDefaultHashtable[zoneCode].ToString().Substring(paramPos * 2, 2), NumberStyles.HexNumber);
                                    byte nacZoneDataValue = zoneDataValueUser;
                                    zoneDataValueUser = ((!((CheckBox)tabControl).Checked) ? ((byte)(zoneDataValueUser & ~paramMask)) : ((byte)(zoneDataValueUser | paramMask)));
                                    if (nacZoneDataValue != zoneDataValueUser)
                                    {
                                        paramsEditedList += zoneCode + ": " + ((JObject)jObject["detail"])[this.LanguageCode].ToString() + "\n";
                                    }
                                    clonedDefaultHashtable[zoneCode] = clonedDefaultHashtable[zoneCode].ToString().Substring(0, paramPos * 2) + zoneDataValueUser.ToString("x2").ToUpper() + clonedDefaultHashtable[zoneCode].ToString().Substring(paramPos * 2 + 2);
                                }
                                catch
                                {
                                    MessageBox.Show(this, "Incorrect value in " + zoneCode + ": " + zoneDescription, "Error");
                                    return;
                                }
                            }
                        }

                        if (tabControl.GetType().ToString().Contains("ComboBox"))
                        {
                            int selectedIndex = ((ComboBox)tabControl).SelectedIndex;
                            jObject = (JObject)((ComboBox)tabControl).Tag;
                            JArray paramListboxJson = jObject["listbox"].Value<JArray>();
                            JObject selectedParamListboxJson = (JObject)paramListboxJson[0];

                            if (selectedIndex < paramListboxJson.Count)
                            {
                                selectedParamListboxJson = (JObject)paramListboxJson[selectedIndex];
                            }
                            else
                            {
                                selectedParamListboxJson["value"] = jObject["unknown"];
                            }

                            byte selectedParamValue = byte.Parse(selectedParamListboxJson["value"].ToString(), NumberStyles.HexNumber);
                            paramMask = byte.Parse(jObject["mask"].ToString(), NumberStyles.HexNumber);
                            zoneCode = jObject["zone"].ToString();
                            paramPos = int.Parse(jObject["pos"].ToString()) - 4;

                            if (clonedDefaultHashtable[zoneCode].ToString().Length < 20 && zoneCode == "2108" && regex.Match(jObject["name"].ToString()).Success)
                            {
                                paramPos--;
                            }

                            byte nacZoneDataValue = byte.Parse(clonedDefaultHashtable[zoneCode].ToString().Substring(paramPos * 2, 2), NumberStyles.HexNumber);
                            byte userZoneDataValue = nacZoneDataValue;
                            userZoneDataValue = (byte)(userZoneDataValue & ~paramMask);
                            userZoneDataValue = (byte)(userZoneDataValue | selectedParamValue);

                            if (userZoneDataValue != nacZoneDataValue)
                            {
                                paramsEditedList += zoneCode + ": " + ((JObject)jObject["detail"])[this.LanguageCode].ToString() + "\n";
                            }

                            clonedDefaultHashtable[zoneCode] = clonedDefaultHashtable[zoneCode].ToString().Substring(0, paramPos * 2) + userZoneDataValue.ToString("x2").ToUpper() + clonedDefaultHashtable[zoneCode].ToString().Substring(paramPos * 2 + 2);
                        }

                        if (!tabControl.GetType().ToString().Contains("TextBox") || tabControl.Tag.GetType().ToString().Contains("String"))
                        {
                            continue;
                        }

                        jObject = (JObject)((TextBox)tabControl).Tag;
                        int paramUserValue = 0;
                        paramMask = byte.Parse(jObject["mask"].ToString(), NumberStyles.HexNumber);
                        zoneCode = jObject["zone"].ToString();
                        paramPos = jObject["maskBinary"].ToString().LastIndexOf('1');
                        int paramSize = int.Parse(jObject["size"].ToString());
                        paramPos = 8 - paramPos - 1;
                        byte paramMaxDigit = (byte)(paramMask >> paramPos);

                        try
                        {
                            if (jObject["name"].ToString() == "DSZ")
                            {
                                int value = (int)((double.Parse(((TextBox)tabControl).Text.Replace(".", ",")) - 3.0) * 2.0);
                                paramUserValue = (byte)Convert.ToInt16(value);
                                paramUserValue += 86;
                            }
                            else
                            {
                                int value2 = int.Parse(((TextBox)tabControl).Text);
                                paramUserValue = Convert.ToInt16(value2);
                            }
                        }
                        catch
                        {
                        }

                        double paramMaxValue = Math.Pow(unchecked((int)paramMaxDigit), paramSize);

                        if (paramUserValue >= 0 && (double)paramUserValue <= paramMaxValue)
                        {
                            if (paramSize <= 1)
                            {
                                paramUserValue = (byte)(paramUserValue << paramPos);
                            }

                            paramPos = int.Parse(jObject["pos"].ToString()) - 4;

                            if (clonedDefaultHashtable[zoneCode].ToString().Length < 20 && zoneCode == "2108" && regex.Match(jObject["name"].ToString()).Success)
                            {
                                paramPos--;
                            }

                            int nacZoneDataValue = int.Parse(clonedDefaultHashtable[zoneCode].ToString().Substring(paramPos * 2, paramSize * 2), NumberStyles.HexNumber);
                            int userZoneDataValue = nacZoneDataValue;
                            if (paramSize > 1)
                            {
                                userZoneDataValue = paramUserValue;
                            }
                            else
                            {
                                userZoneDataValue = (byte)(userZoneDataValue & ~paramMask);
                                userZoneDataValue |= paramUserValue;
                            }
                            if (nacZoneDataValue != userZoneDataValue)
                            {
                                paramsEditedList += zoneCode + ": " + ((JObject)jObject["detail"])[this.LanguageCode].ToString() + "\n";
                            }
                            clonedDefaultHashtable[zoneCode] = clonedDefaultHashtable[zoneCode].ToString().Substring(0, paramPos * 2) + userZoneDataValue.ToString("x2").ToUpper().PadLeft(paramSize * 2, '0') + clonedDefaultHashtable[zoneCode].ToString().Substring(paramPos * 2 + paramSize * 2);
                        }
                    }
                }

                // Clear previous user data
                this._UserZoneValueHash.Clear();

                // Copy to output hash
                foreach (DictionaryEntry nacZoneKeyValue in this.NacZoneValueHash)
                {
                    if (clonedDefaultHashtable.ContainsKey(nacZoneKeyValue.Key) && nacZoneKeyValue.Value.ToString() != clonedDefaultHashtable[nacZoneKeyValue.Key].ToString())
                    {
                        this._UserZoneValueHash.Add(nacZoneKeyValue.Key, clonedDefaultHashtable[nacZoneKeyValue.Key]);
                    }
                }

                paramsEditedList += "\r\nConfirm your change(s) ?";
                if (this._UserZoneValueHash.Count > 0)
                {
                    if (MessageBox.Show(this, paramsEditedList, "", MessageBoxButtons.YesNo) != DialogResult.No)
                    {
                        base.DialogResult = DialogResult.OK;
                        Close();
                    }
                }
                else
                {
                    MessageBox.Show(this, "No parameters modified", "", MessageBoxButtons.OK);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.TabPage1 = new System.Windows.Forms.TabPage();
            this.LabelZoneSummary2 = new System.Windows.Forms.Label();
            this.LabelZoneSummary1 = new System.Windows.Forms.Label();
            this.TextBoxBtName = new System.Windows.Forms.TextBox();
            this.LabelBtName = new System.Windows.Forms.Label();
            this.TextBoxSN = new System.Windows.Forms.TextBox();
            this.TextBoxVin = new System.Windows.Forms.TextBox();
            this.LabelNacSerial = new System.Windows.Forms.Label();
            this.LabelVin = new System.Windows.Forms.Label();
            this.ButtonSave = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).BeginInit();
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            this.TabControl1.SuspendLayout();
            this.TabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SplitContainer1
            // 
            this.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.SplitContainer1.IsSplitterFixed = true;
            this.SplitContainer1.Location = new System.Drawing.Point(0, 0);
            this.SplitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.SplitContainer1.Name = "SplitContainer1";
            this.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer1.Panel1
            // 
            this.SplitContainer1.Panel1.AutoScroll = true;
            this.SplitContainer1.Panel1.Controls.Add(this.TabControl1);
            // 
            // SplitContainer1.Panel2
            // 
            this.SplitContainer1.Panel2.Controls.Add(this.ButtonSave);
            this.SplitContainer1.Panel2.Controls.Add(this.ButtonCancel);
            this.SplitContainer1.Size = new System.Drawing.Size(1060, 763);
            this.SplitContainer1.SplitterDistance = 725;
            this.SplitContainer1.SplitterWidth = 5;
            this.SplitContainer1.TabIndex = 0;
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.TabPage1);
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Multiline = true;
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.ShowToolTips = true;
            this.TabControl1.Size = new System.Drawing.Size(1060, 725);
            this.TabControl1.TabIndex = 13;
            // 
            // TabPage1
            // 
            this.TabPage1.AutoScroll = true;
            this.TabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.TabPage1.Controls.Add(this.LabelZoneSummary2);
            this.TabPage1.Controls.Add(this.LabelZoneSummary1);
            this.TabPage1.Controls.Add(this.TextBoxBtName);
            this.TabPage1.Controls.Add(this.LabelBtName);
            this.TabPage1.Controls.Add(this.TextBoxSN);
            this.TabPage1.Controls.Add(this.TextBoxVin);
            this.TabPage1.Controls.Add(this.LabelNacSerial);
            this.TabPage1.Controls.Add(this.LabelVin);
            this.TabPage1.Location = new System.Drawing.Point(4, 24);
            this.TabPage1.Name = "TabPage1";
            this.TabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage1.Size = new System.Drawing.Size(1052, 697);
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "Main";
            // 
            // LabelZoneSummary2
            // 
            this.LabelZoneSummary2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.LabelZoneSummary2.Location = new System.Drawing.Point(535, 83);
            this.LabelZoneSummary2.Name = "LabelZoneSummary2";
            this.LabelZoneSummary2.Size = new System.Drawing.Size(503, 533);
            this.LabelZoneSummary2.TabIndex = 14;
            // 
            // LabelZoneSummary1
            // 
            this.LabelZoneSummary1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.LabelZoneSummary1.Location = new System.Drawing.Point(18, 83);
            this.LabelZoneSummary1.Name = "LabelZoneSummary1";
            this.LabelZoneSummary1.Size = new System.Drawing.Size(503, 533);
            this.LabelZoneSummary1.TabIndex = 13;
            // 
            // TextBoxBtName
            // 
            this.TextBoxBtName.Location = new System.Drawing.Point(634, 11);
            this.TextBoxBtName.Margin = new System.Windows.Forms.Padding(4);
            this.TextBoxBtName.Name = "TextBoxBtName";
            this.TextBoxBtName.Size = new System.Drawing.Size(199, 23);
            this.TextBoxBtName.TabIndex = 12;
            this.TextBoxBtName.Tag = "BTN";
            this.TextBoxBtName.Visible = false;
            // 
            // LabelBtName
            // 
            this.LabelBtName.Location = new System.Drawing.Point(535, 11);
            this.LabelBtName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelBtName.Name = "LabelBtName";
            this.LabelBtName.Size = new System.Drawing.Size(102, 26);
            this.LabelBtName.TabIndex = 11;
            this.LabelBtName.Text = "Bluetooth Name:";
            this.LabelBtName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LabelBtName.Visible = false;
            // 
            // TextBoxSN
            // 
            this.TextBoxSN.Location = new System.Drawing.Point(143, 41);
            this.TextBoxSN.Margin = new System.Windows.Forms.Padding(4);
            this.TextBoxSN.Name = "TextBoxSN";
            this.TextBoxSN.ReadOnly = true;
            this.TextBoxSN.Size = new System.Drawing.Size(182, 23);
            this.TextBoxSN.TabIndex = 9;
            // 
            // TextBoxVin
            // 
            this.TextBoxVin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TextBoxVin.Location = new System.Drawing.Point(143, 11);
            this.TextBoxVin.Margin = new System.Windows.Forms.Padding(4);
            this.TextBoxVin.Name = "TextBoxVin";
            this.TextBoxVin.Size = new System.Drawing.Size(182, 20);
            this.TextBoxVin.TabIndex = 10;
            // 
            // LabelNacSerial
            // 
            this.LabelNacSerial.Location = new System.Drawing.Point(18, 41);
            this.LabelNacSerial.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelNacSerial.Name = "LabelNacSerial";
            this.LabelNacSerial.Size = new System.Drawing.Size(116, 26);
            this.LabelNacSerial.TabIndex = 1;
            this.LabelNacSerial.Text = "NAC serial number:";
            this.LabelNacSerial.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelVin
            // 
            this.LabelVin.Location = new System.Drawing.Point(18, 15);
            this.LabelVin.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelVin.Name = "LabelVin";
            this.LabelVin.Size = new System.Drawing.Size(116, 26);
            this.LabelVin.TabIndex = 0;
            this.LabelVin.Text = "VIN:";
            this.LabelVin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ButtonSave
            // 
            this.ButtonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonSave.Location = new System.Drawing.Point(968, 4);
            this.ButtonSave.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(88, 26);
            this.ButtonSave.TabIndex = 1;
            this.ButtonSave.Text = "Save";
            this.ButtonSave.UseVisualStyleBackColor = true;
            this.ButtonSave.Click += new System.EventHandler(this.OnButtonSaveClick);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(872, 4);
            this.ButtonCancel.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(88, 26);
            this.ButtonCancel.TabIndex = 0;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ParamsForm
            // 
            this.AcceptButton = this.ButtonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(1060, 763);
            this.ControlBox = false;
            this.Controls.Add(this.SplitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1076, 779);
            this.Name = "ParamsForm";
            this.ShowInTaskbar = false;
            this.Text = "Parameters";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).EndInit();
            this.SplitContainer1.ResumeLayout(false);
            this.TabControl1.ResumeLayout(false);
            this.TabPage1.ResumeLayout(false);
            this.TabPage1.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}