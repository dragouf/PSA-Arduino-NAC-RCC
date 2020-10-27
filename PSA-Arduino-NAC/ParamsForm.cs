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

namespace PSA_Arduino_NAC
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

		private Label Label1;

		private Label Label2;

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

					List<string> zonesFlatten = new List<string>();

					foreach (JProperty jsonZone in jsonObject.Root["NAC"]["zones"].Children())
					{
						string zoneNodeCode = jsonZone.Name;
						if (jsonZone.Name.Contains("0106") || jsonZone.Name.Contains("F190") || jsonZone.Name.Contains("F18C") || !this.NacZoneValueHash.ContainsKey(zoneNodeCode))
						{
							continue;
						}

						zonesFlatten.Add(zoneNodeCode + ": " + jsonZone.Value["name"].ToString() + "\n");

                        TabPage tabPage = new TabPage(zoneNodeCode)
                        {
                            AutoScroll = true,
                            BackColor = SystemColors.Control,
                            Padding = new Padding(3),
                            TabIndex = 0,
                            Name = jsonZone.Name
                        };

                        int currentYPos = labelYPosStart;
						paramIndex = 0;

                        Label labelZoneCode = new Label
                        {
                            Location = new Point(labelXPosStart, currentYPos + paramIndex * 30),
                            Name = jsonZone.Value["name"].ToString(),
                            Text = zoneNodeCode + ": " + jsonZone.Value["name"].ToString(),
                            Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold, GraphicsUnit.Point, 0),
                            Size = new Size(350, 25),
                            TabIndex = 4
                        };

                        tabPage.Controls.Add(labelZoneCode);

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
                                    checkBox.CheckedChanged += OnCheckboxChanged;
		

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

					for (int i = 0; i < zonesFlatten.Count; i++)
					{
						if (i < unchecked(zonesFlatten.Count / 2))
						{
							Label2.Text += zonesFlatten[i].ToString();
						}
						else
						{
							Label1.Text += zonesFlatten[i].ToString();
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
			this.components = new Container();

			this.SplitContainer1 = new SplitContainer();
			this.TabControl1 = new TabControl();
			this.TabPage1 = new TabPage();
			Label1 = new Label();
			Label2 = new Label();
			this.TextBoxBtName = new TextBox();
			this.LabelBtName = new Label();
			this.TextBoxSN = new TextBox();
			this.TextBoxVin = new TextBox();
			this.LabelNacSerial = new Label();
			this.LabelVin = new Label();
			this.ButtonSave = new Button();
			this.ButtonCancel = new Button();

			((ISupportInitialize)this.SplitContainer1).BeginInit();

			this.SplitContainer1.Panel1.SuspendLayout();
			this.SplitContainer1.Panel2.SuspendLayout();
			this.SplitContainer1.SuspendLayout();
			this.TabControl1.SuspendLayout();
			this.TabPage1.SuspendLayout();

			SuspendLayout();

			this.SplitContainer1.Dock = DockStyle.Fill;
			this.SplitContainer1.FixedPanel = FixedPanel.Panel2;
			this.SplitContainer1.IsSplitterFixed = true;
			this.SplitContainer1.Location = new Point(0, 0);
			this.SplitContainer1.Margin = new Padding(4);
			this.SplitContainer1.Name = "splitContainer1";
			this.SplitContainer1.Orientation = Orientation.Horizontal;
			this.SplitContainer1.Panel1.AutoScroll = true;
			this.SplitContainer1.Panel1.Controls.Add(this.TabControl1);
			this.SplitContainer1.Panel2.Controls.Add(this.ButtonSave);
			this.SplitContainer1.Panel2.Controls.Add(this.ButtonCancel);
			this.SplitContainer1.Size = new Size(1209, 810);
			this.SplitContainer1.SplitterDistance = 753;
			this.SplitContainer1.SplitterWidth = 5;
			this.SplitContainer1.TabIndex = 0;

			this.TabControl1.Controls.Add(this.TabPage1);
			this.TabControl1.Dock = DockStyle.Fill;
			this.TabControl1.Location = new Point(0, 0);
			this.TabControl1.Multiline = true;
			this.TabControl1.Name = "tabControl1";
			this.TabControl1.RightToLeft = RightToLeft.No;
			this.TabControl1.SelectedIndex = 0;
			this.TabControl1.Size = new Size(1209, 753);
			this.TabControl1.TabIndex = 13;

			this.TabPage1.AutoScroll = true;
			this.TabPage1.BackColor = SystemColors.Control;
			this.TabPage1.Controls.Add(Label1);
			this.TabPage1.Controls.Add(Label2);
			this.TabPage1.Controls.Add(this.TextBoxBtName);
			this.TabPage1.Controls.Add(this.LabelBtName);
			this.TabPage1.Controls.Add(this.TextBoxSN);
			this.TabPage1.Controls.Add(this.TextBoxVin);
			this.TabPage1.Controls.Add(this.LabelNacSerial);
			this.TabPage1.Controls.Add(this.LabelVin);
			this.TabPage1.Location = new Point(4, 25);
			this.TabPage1.Name = "tabPage1";
			this.TabPage1.Padding = new Padding(3);
			this.TabPage1.Size = new Size(1201, 724);
			this.TabPage1.TabIndex = 0;
			this.TabPage1.Text = "Main";

			Label1.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold, GraphicsUnit.Point, 0);
			Label1.Location = new Point(611, 89);
			Label1.Name = "label4";
			Label1.Size = new Size(575, 569);
			Label1.TabIndex = 14;

			Label2.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold, GraphicsUnit.Point, 0);
			Label2.Location = new Point(21, 89);
			Label2.Name = "label3";
			Label2.Size = new Size(575, 569);
			Label2.TabIndex = 13;

			this.TextBoxBtName.Location = new Point(724, 12);
			this.TextBoxBtName.Margin = new Padding(4);
			this.TextBoxBtName.Name = "textBoxBTName";
			this.TextBoxBtName.Size = new Size(227, 22);
			this.TextBoxBtName.TabIndex = 12;
			this.TextBoxBtName.Tag = "BTN";
			this.TextBoxBtName.Visible = false;

			this.LabelBtName.Location = new Point(611, 12);
			this.LabelBtName.Margin = new Padding(4, 0, 4, 0);
			this.LabelBtName.Name = "labelBTName";
			this.LabelBtName.Size = new Size(117, 28);
			this.LabelBtName.TabIndex = 11;
			this.LabelBtName.Text = "Bluetooth Name:";
			this.LabelBtName.TextAlign = ContentAlignment.MiddleLeft;
			this.LabelBtName.Visible = false;

			this.TextBoxSN.Location = new Point(163, 44);
			this.TextBoxSN.Margin = new Padding(4);
			this.TextBoxSN.Name = "textBoxSN";
			this.TextBoxSN.ReadOnly = true;
			this.TextBoxSN.Size = new Size(208, 22);
			this.TextBoxSN.TabIndex = 9;

			this.TextBoxVin.Location = new Point(163, 12);
			this.TextBoxVin.Margin = new Padding(4);
			this.TextBoxVin.Name = "textBoxVIN";
			this.TextBoxVin.Size = new Size(208, 22);
			this.TextBoxVin.TabIndex = 10;
			this.TextBoxVin.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);

			this.LabelNacSerial.Location = new Point(21, 44);
			this.LabelNacSerial.Margin = new Padding(4, 0, 4, 0);
			this.LabelNacSerial.Name = "label2";
			this.LabelNacSerial.Size = new Size(133, 28);
			this.LabelNacSerial.TabIndex = 1;
			this.LabelNacSerial.Text = "NAC serial number:";
			this.LabelNacSerial.TextAlign = ContentAlignment.MiddleLeft;

			this.LabelVin.Location = new Point(21, 16);
			this.LabelVin.Margin = new Padding(4, 0, 4, 0);
			this.LabelVin.Name = "label1";
			this.LabelVin.Size = new Size(133, 28);
			this.LabelVin.TabIndex = 0;
			this.LabelVin.Text = "VIN:";
			this.LabelVin.TextAlign = ContentAlignment.MiddleLeft;

			this.ButtonSave.Location = new Point(840, 11);
			this.ButtonSave.Margin = new Padding(4);
			this.ButtonSave.Name = "buttonSave";
			this.ButtonSave.Size = new Size(100, 28);
			this.ButtonSave.TabIndex = 1;
			this.ButtonSave.Text = "Save";
			this.ButtonSave.UseVisualStyleBackColor = true;
			this.ButtonSave.Click += OnButtonSaveClick;

			this.ButtonCancel.DialogResult = DialogResult.Cancel;
			this.ButtonCancel.Location = new Point(265, 11);
			this.ButtonCancel.Margin = new Padding(4);
			this.ButtonCancel.Name = "buttonCancel";
			this.ButtonCancel.Size = new Size(100, 28);
			this.ButtonCancel.TabIndex = 0;
			this.ButtonCancel.Text = "Cancel";
			this.ButtonCancel.UseVisualStyleBackColor = true;

			// Form
			base.AcceptButton = this.ButtonSave;
			base.AutoScaleDimensions = new SizeF(8f, 16f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.AutoSize = true;
			base.CancelButton = this.ButtonCancel;
			base.ClientSize = new Size(1209, 810);
			base.ControlBox = false;
			base.Controls.Add(this.SplitContainer1);
			base.Margin = new Padding(4);
			this.MinimumSize = new Size(1227, 828);
			base.Name = "MainParams";
			base.ShowInTaskbar = false;
			this.Text = "Parameters";
			base.Load += OnFormLoad;

			this.SplitContainer1.Panel1.ResumeLayout(false);
			this.SplitContainer1.Panel2.ResumeLayout(false);

			((ISupportInitialize)this.SplitContainer1).EndInit();

			this.SplitContainer1.ResumeLayout(false);
			this.TabControl1.ResumeLayout(false);
			this.TabPage1.ResumeLayout(false);
			this.TabPage1.PerformLayout();
			ResumeLayout(false);
		}
	}
}