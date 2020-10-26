using System;
using System.Collections;
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
		private Hashtable _UserKeyValueHash = new Hashtable(); 

		private Hashtable DefaultKeyValueHash;

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
			return this._UserKeyValueHash;
		}

		public ParamsForm()
		{
			InitializeComponent();
			if (!CultureInfo.InstalledUICulture.NativeName.Contains("fran"))
			{
				this.LanguageCode = "en";
			}
		}

		private void OnFormLoad(object P_0, EventArgs P_1)
		{
		}

		public void InitForm(ref Hashtable defaultKeyValueHash, ref JObject P_1)
		{
			this.DefaultKeyValueHash = defaultKeyValueHash;
			checked
			{
				if (defaultKeyValueHash.ContainsKey("F190") && defaultKeyValueHash.ContainsKey("F18C") && defaultKeyValueHash.ContainsKey("0106"))
				{
					string text = defaultKeyValueHash["F190"].ToString();
					if (text != "" && text != "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF")
					{
						for (int i = 0; i < text.Length; i += 2)
						{
							this.TextBoxVin.Text += Convert.ToChar(byte.Parse(text.Substring(i, 2), NumberStyles.HexNumber));
						}
						this.TextBoxVin.Tag = text;
					}
					else
					{
						this.TextBoxVin.Text = "?????????????????";
						this.TextBoxVin.Tag = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
					}
					Label label = this.LabelBtName;
					bool visible = (this.TextBoxBtName.Visible = true);
					label.Visible = visible;
					this.TextBoxBtName.Tag = defaultKeyValueHash["0106"].ToString();
					text = defaultKeyValueHash["0106"].ToString().Replace("00", "");
					for (int i = 0; i < text.Length; i += 2)
					{
						this.TextBoxBtName.Text += Convert.ToChar(byte.Parse(text.Substring(i, 2), NumberStyles.HexNumber));
					}
					text = defaultKeyValueHash["F18C"].ToString();
					if (text != null)
					{
						for (int i = 0; i < text.Length; i += 2)
						{
							this.TextBoxSN.Text += Convert.ToChar(byte.Parse(text.Substring(i, 2), NumberStyles.HexNumber));
						}
						this.TextBoxSN.Tag = this.TextBoxSN.Text;
					}
					else
					{
						this.TextBoxSN.Tag = "";
					}
					int num = 16;
					int num2 = 21;
					Regex regex = new Regex("(DZK|DZR|E5B|E88|EIO)");
					int num3 = 0;
					ArrayList arrayList = new ArrayList();
					foreach (JProperty item in P_1.Root["NAC"]["zones"].Children())
					{
						string name = item.Name;
						if (item.Name.Contains("0106") || item.Name.Contains("F190") || item.Name.Contains("F18C") || !this.DefaultKeyValueHash.ContainsKey(name))
						{
							continue;
						}
						arrayList.Add(name + ": " + item.Value["name"].ToString() + "\n");
						TabPage tabPage = new TabPage(name);
						tabPage.AutoScroll = true;
						tabPage.BackColor = SystemColors.Control;
						tabPage.Padding = new Padding(3);
						tabPage.TabIndex = 0;
						tabPage.Name = item.Name;
						int num4 = num2;
						num3 = 0;
						Label label2 = new Label();
						label2.Location = new Point(num, num4 + num3 * 30);
						label2.Name = item.Value["name"].ToString();
						label2.Text = name + ": " + item.Value["name"].ToString();
						label2.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold, GraphicsUnit.Point, 0);
						label2.Size = new Size(350, 25);
						label2.TabIndex = 4;
						tabPage.Controls.Add(label2);
						num4 += 30;
						foreach (JObject item2 in item.Value["params"].Children())
						{
							Label label3 = new Label();
							label3.Location = new Point(num, num4 + num3 * 30);
							label3.Name = item2["extra_name"].ToString();
							label3.Text = item2["name"].ToString() + ": " + item2["extra_name"].ToString();
							if (((JObject)item2["detail"])[this.LanguageCode].ToString() != "")
							{
								label3.Text = ((JObject)item2["detail"])[this.LanguageCode].ToString();
							}
							label3.Size = new Size(310, 32);
							label3.TabIndex = 4;
							tabPage.Controls.Add(label3);
							int num5 = 0;
							if (item2["listbox"] != null)
							{
								ComboBox comboBox = new ComboBox();
								comboBox.BackColor = Color.White;
								comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
								comboBox.FormattingEnabled = true;
								comboBox.Location = new Point(num + 315, num4 + num3 * 30 - 5);
								comboBox.Name = "comboBox1";
								comboBox.Size = new Size(400, 21);
								comboBox.TabIndex = 8;
								tabPage.Controls.Add(comboBox);
								if (defaultKeyValueHash.ContainsKey(name))
								{
									text = defaultKeyValueHash[name].ToString();
									if (text != "")
									{
										int num6 = int.Parse(item2["pos"].ToString()) - 4;
										if (text.Length < 20 && name == "2108" && regex.Match(item2["name"].ToString()).Success)
										{
											num6--;
										}
										num5 = byte.Parse(text.Substring(num6 * 2, 2), NumberStyles.HexNumber);
										byte b = byte.Parse(item2["mask"].ToString(), NumberStyles.HexNumber);
										num5 &= b;
									}
								}
								foreach (JObject item3 in item2["listbox"].Children())
								{
									comboBox.Items.Add(((JObject)item3["text"])[this.LanguageCode].ToString());
									if (item3["value"].ToString() == num5.ToString("X2"))
									{
										comboBox.SelectedIndex = comboBox.Items.Count - 1;
									}
								}
								JObject jObject3 = (JObject)item2.DeepClone();
								jObject3.Add(new JProperty("zone", name));
								if (comboBox.SelectedIndex == -1)
								{
									comboBox.Items.Add("Unknown 0x" + num5.ToString("X2"));
									comboBox.SelectedIndex = comboBox.Items.Count - 1;
									jObject3.Add(new JProperty("unknown", num5.ToString("X2")));
								}
								comboBox.Tag = jObject3;
							}
							else if (item2["mask"].ToString() == "FF")
							{
								string text2 = "??";
								string text3 = "";
								if (defaultKeyValueHash.ContainsKey(name))
								{
									text = defaultKeyValueHash[name].ToString();
									if (text != "")
									{
										int num6 = int.Parse(item2["pos"].ToString()) - 4;
										int length = int.Parse(item2["size"].ToString()) * 2;
										if (text.Length < 20 && name == "2108" && regex.Match(item2["name"].ToString()).Success)
										{
											num6--;
										}
										num5 = int.Parse(text.Substring(num6 * 2, length), NumberStyles.HexNumber);
										text2 = ((!(item2["name"].ToString() == "DSZ")) ? Convert.ToInt32(num5).ToString() : (3.0 + (double)(num5 - 86) * 0.5).ToString("#.#"));
										if (item2["unit"] != null)
										{
											text3 = item2["unit"].ToString();
										}
									}
								}
								TextBox textBox = new TextBox();
								textBox.Location = new Point(num + 315, num4 + num3 * 30);
								textBox.Name = "valeur";
								textBox.Text = text2;
								textBox.Size = new Size(30, 23);
								textBox.TabIndex = 4;
								JObject jObject3 = (JObject)item2.DeepClone();
								jObject3.Add(new JProperty("zone", name));
								textBox.Tag = jObject3;
								tabPage.Controls.Add(textBox);
								Label label4 = new Label();
								label4.Location = new Point(num + 355, num4 + num3 * 30);
								label4.Name = "unit";
								label4.Text = text3;
								label4.Size = new Size(300, 23);
								label4.TabIndex = 4;
								tabPage.Controls.Add(label4);
							}
							else if (item2["maskBinary"].ToString().Contains("11"))
							{
								text = defaultKeyValueHash[name].ToString();
								int num6 = int.Parse(item2["pos"].ToString()) - 4;
								if (text.Length < 20 && name == "2108" && regex.Match(item2["name"].ToString()).Success)
								{
									num6--;
								}
								num5 = byte.Parse(text.Substring(num6 * 2, 2), NumberStyles.HexNumber);
								byte b = byte.Parse(item2["mask"].ToString(), NumberStyles.HexNumber);
								num5 &= b;
								num6 = item2["maskBinary"].ToString().LastIndexOf('1');
								num6 = 8 - num6 - 1;
								num5 = (byte)(num5 >> num6);
								TextBox textBox = new TextBox();
								textBox.Location = new Point(num + 315, num4 + num3 * 30);
								textBox.Name = ((JObject)item2["detail"])[this.LanguageCode].ToString();
								textBox.Text = Convert.ToInt32(num5).ToString();
								textBox.Size = new Size(30, 23);
								textBox.TabIndex = 4;
								JObject jObject3 = (JObject)item2.DeepClone();
								jObject3.Add(new JProperty("zone", name));
								textBox.Tag = jObject3;
								tabPage.Controls.Add(textBox);
							}
							else
							{
								text = defaultKeyValueHash[name].ToString();
								int num6 = int.Parse(item2["pos"].ToString()) - 4;
								if (text.Length < 20 && name == "2108" && regex.Match(item2["name"].ToString()).Success)
								{
									num6--;
								}
								if (num6 * 2 < text.Length)
								{
									JObject jObject3 = (JObject)item2.DeepClone();
									jObject3.Add(new JProperty("zone", name));
									
									CheckBox checkBox = new CheckBox();
									checkBox.Location = new Point(num + 315, num4 + num3 * 30 - 5);
									checkBox.Name = item2["name"].ToString();
									checkBox.Tag = jObject3;
									checkBox.Text = "Disabled";
									checkBox.CheckedChanged += OnCheckboxChanged;
									checkBox.Size = new Size(95, 24);
									checkBox.TabIndex = 5;
									checkBox.UseVisualStyleBackColor = true;

									tabPage.Controls.Add(checkBox);
									if (defaultKeyValueHash.ContainsKey(name) && text != "")
									{
										num5 = byte.Parse(text.Substring(num6 * 2, 2), NumberStyles.HexNumber);
										byte b = byte.Parse(item2["mask"].ToString(), NumberStyles.HexNumber);
										if ((num5 & b) == b)
										{
											checkBox.Checked = true;
										}
									}
									Label label5 = new Label();
									label5.Location = new Point(num + 410, num4 + num3 * 30);
									label5.Name = ((JObject)item2["detail"])[this.LanguageCode].ToString();
									label5.Text = "";
									label5.Size = new Size(400, 23);
									label5.TabIndex = 4;
									tabPage.Controls.Add(label5);
								}
								else
								{
									Label label5 = new Label();
									label5.Location = new Point(num + 410, num4 + num3 * 30);
									label5.Name = ((JObject)item2["detail"])[this.LanguageCode].ToString();
									label5.Text = "Unsupported by your unit";
									label5.Size = new Size(400, 23);
									label5.TabIndex = 4;
									tabPage.Controls.Add(label5);
								}
							}
							Label label6 = new Label();
							label6.Location = new Point(num + 815, num4 + num3 * 30);
							label6.Name = "valhexa";
							label6.Text = num5.ToString("X2");
							label6.Size = new Size(50, 32);
							label6.TabIndex = 4;
							tabPage.Controls.Add(label6);
							num3++;
						}
						this.TabControl1.TabPages.Add(tabPage);
					}
					for (int i = 0; i < arrayList.Count; i++)
					{
						if (i < unchecked(arrayList.Count / 2))
						{
							Label2.Text += arrayList[i].ToString();
						}
						else
						{
							Label1.Text += arrayList[i].ToString();
						}
					}
				}
				else
				{
					MessageBox.Show(this, "Missing zones, please make a new reading - Disconnect PSA Diag interface if plugged", "Error");
				}
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

		private void OnCheckboxChanged(object P_0, EventArgs P_1)
		{
			if (((CheckBox)P_0).Checked)
			{
				((CheckBox)P_0).Text = "Enabled";
			}
			else
			{
				((CheckBox)P_0).Text = "Disabled";
			}
		}

		private void OnButtonSaveClick(object P_0, EventArgs P_1)
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
				foreach (TabPage tabPage2 in this.TabControl1.TabPages)
				{
					foreach (Control control2 in tabPage2.Controls)
					{
						if (!control2.GetType().ToString().Contains("TextBox") || control2.Tag.GetType().ToString().Contains("String"))
						{
							continue;
						}
						if (((TextBox)control2).Text.Contains("?"))
						{
							((TextBox)control2).Text = "0";
						}
						JObject jObject = (JObject)((TextBox)control2).Tag;
						string text = jObject["zone"].ToString();
						string text2 = jObject["name"].ToString() + ": " + jObject["extra_name"].ToString();
						if (((JObject)jObject["detail"])[this.LanguageCode].ToString() != "")
						{
							text2 = ((JObject)jObject["detail"])[this.LanguageCode].ToString();
						}
						int num = 0;
						byte b = byte.Parse(jObject["mask"].ToString(), NumberStyles.HexNumber);
						int num2 = jObject["maskBinary"].ToString().LastIndexOf('1');
						num2 = 8 - num2 - 1;
						byte b2 = (byte)(b >> num2);
						int num3 = int.Parse(jObject["size"].ToString());
						try
						{
							if (jObject["name"].ToString() == "DSZ")
							{
								int value = (int)((double.Parse(((TextBox)control2).Text.Replace(".", ",")) - 3.0) * 2.0);
								num = Convert.ToByte(value);
								num += 86;
							}
							else
							{
								int value2 = int.Parse(((TextBox)control2).Text);
								num = Convert.ToInt16(value2);
							}
							unchecked
							{
								if ((double)num > Math.Pow((int)b2, num3))
								{
									MessageBox.Show(this, "Value greater than maximum value " + Convert.ToInt32(Math.Pow((int)b2, num3)) + " in " + text + ": " + text2, "Error");
									return;
								}
							}
						}
						catch
						{
							MessageBox.Show(this, "Incorrect value in " + text + ": " + text2, "Error");
							return;
						}
					}
				}
				string text3 = "Parameters's zones are modified:\r\n";
				var clonedDefaultHashtable = (Hashtable)this.DefaultKeyValueHash.Clone();
				string text4 = "";
				char[] array = this.TextBoxVin.Text.ToCharArray();
				for (int i = 0; i < array.Length; i++)
				{
					text4 = ((array[i] != '?') ? (text4 + Convert.ToByte(array[i]).ToString("x2")) : (text4 + "FF"));
				}
				clonedDefaultHashtable["F190"] = text4.ToUpper();
				if (this.TextBoxVin.Tag.ToString() != clonedDefaultHashtable["F190"].ToString())
				{
					text3 += "F190: VIN\r\n";
				}
				if (this.TextBoxBtName.Visible)
				{
					text4 = "";
					array = this.TextBoxBtName.Text.ToCharArray();
					for (int i = 0; i < array.Length; i++)
					{
						text4 += Convert.ToByte(array[i]).ToString("x2");
					}
					int num4 = 60 - text4.Length;
					for (int i = 0; i < num4; i++)
					{
						text4 += "0";
					}
					clonedDefaultHashtable["0106"] = text4.ToUpper();
					if (this.TextBoxBtName.Tag.ToString() != clonedDefaultHashtable["0106"].ToString())
					{
						text3 += "0106: Bluetooth Name\r\n";
					}
				}
				foreach (TabPage tabPage3 in this.TabControl1.TabPages)
				{
					foreach (Control control3 in tabPage3.Controls)
					{
						JObject jObject;
						byte b;
						string text;
						int num2;
						if (control3.GetType().ToString().Contains("CheckBox"))
						{
							jObject = (JObject)((CheckBox)control3).Tag;
							string text2 = jObject["name"].ToString() + ": " + jObject["extra_name"].ToString();
							text = jObject["zone"].ToString();
							b = byte.Parse(jObject["mask"].ToString(), NumberStyles.HexNumber);
							num2 = int.Parse(jObject["pos"].ToString()) - 4;
							if (clonedDefaultHashtable[text].ToString().Length < 20 && text == "2108" && regex.Match(text2).Success)
							{
								num2--;
							}
							if (clonedDefaultHashtable.ContainsKey(text))
							{
								byte b3 = 0;
								try
								{
									b3 = byte.Parse(clonedDefaultHashtable[text].ToString().Substring(num2 * 2, 2), NumberStyles.HexNumber);
									byte b4 = b3;
									b3 = ((!((CheckBox)control3).Checked) ? ((byte)(b3 & ~b)) : ((byte)(b3 | b)));
									if (b4 != b3)
									{
										string text5 = text3;
										text3 = text5 + text + ": " + ((JObject)jObject["detail"])[this.LanguageCode].ToString() + "\n";
									}
									clonedDefaultHashtable[text] = clonedDefaultHashtable[text].ToString().Substring(0, num2 * 2) + b3.ToString("x2").ToUpper() + clonedDefaultHashtable[text].ToString().Substring(num2 * 2 + 2);
								}
								catch
								{
									MessageBox.Show(this, "Incorrect value in " + text + ": " + text2, "Error");
									return;
								}
							}
						}
						if (control3.GetType().ToString().Contains("ComboBox"))
						{
							int selectedIndex = ((ComboBox)control3).SelectedIndex;
							jObject = (JObject)((ComboBox)control3).Tag;
							JArray jArray = jObject["listbox"].Value<JArray>();
							JObject jObject2 = (JObject)jArray[0];
							if (selectedIndex < jArray.Count)
							{
								jObject2 = (JObject)jArray[selectedIndex];
							}
							else
							{
								jObject2["value"] = jObject["unknown"];
							}
							byte b5 = byte.Parse(jObject2["value"].ToString(), NumberStyles.HexNumber);
							b = byte.Parse(jObject["mask"].ToString(), NumberStyles.HexNumber);
							text = jObject["zone"].ToString();
							num2 = int.Parse(jObject["pos"].ToString()) - 4;
							if (clonedDefaultHashtable[text].ToString().Length < 20 && text == "2108" && regex.Match(jObject["name"].ToString()).Success)
							{
								num2--;
							}
							byte b3 = byte.Parse(clonedDefaultHashtable[text].ToString().Substring(num2 * 2, 2), NumberStyles.HexNumber);
							byte b4 = b3;
							b3 = (byte)(b3 & ~b);
							b3 = (byte)(b3 | b5);
							if (b4 != b3)
							{
								string text5 = text3;
								text3 = text5 + text + ": " + ((JObject)jObject["detail"])[this.LanguageCode].ToString() + "\n";
							}
							clonedDefaultHashtable[text] = clonedDefaultHashtable[text].ToString().Substring(0, num2 * 2) + b3.ToString("x2").ToUpper() + clonedDefaultHashtable[text].ToString().Substring(num2 * 2 + 2);
						}
						if (!control3.GetType().ToString().Contains("TextBox") || control3.Tag.GetType().ToString().Contains("String"))
						{
							continue;
						}
						jObject = (JObject)((TextBox)control3).Tag;
						int num = 0;
						b = byte.Parse(jObject["mask"].ToString(), NumberStyles.HexNumber);
						text = jObject["zone"].ToString();
						num2 = jObject["maskBinary"].ToString().LastIndexOf('1');
						int num3 = int.Parse(jObject["size"].ToString());
						num2 = 8 - num2 - 1;
						byte b2 = (byte)(b >> num2);
						try
						{
							if (jObject["name"].ToString() == "DSZ")
							{
								int value = (int)((double.Parse(((TextBox)control3).Text.Replace(".", ",")) - 3.0) * 2.0);
								num = (byte)Convert.ToInt16(value);
								num += 86;
							}
							else
							{
								int value2 = int.Parse(((TextBox)control3).Text);
								num = Convert.ToInt16(value2);
							}
						}
						catch
						{
						}
						if (num >= 0 && (double)num <= Math.Pow(unchecked((int)b2), num3))
						{
							if (num3 <= 1)
							{
								num = (byte)(num << num2);
							}
							num2 = int.Parse(jObject["pos"].ToString()) - 4;
							if (clonedDefaultHashtable[text].ToString().Length < 20 && text == "2108" && regex.Match(jObject["name"].ToString()).Success)
							{
								num2--;
							}
							int num5 = int.Parse(clonedDefaultHashtable[text].ToString().Substring(num2 * 2, num3 * 2), NumberStyles.HexNumber);
							int num6 = num5;
							if (num3 > 1)
							{
								num5 = num;
							}
							else
							{
								num5 = (byte)(num5 & ~b);
								num5 |= num;
							}
							if (num6 != num5)
							{
								string text5 = text3;
								text3 = text5 + text + ": " + ((JObject)jObject["detail"])[this.LanguageCode].ToString() + "\n";
							}
							clonedDefaultHashtable[text] = clonedDefaultHashtable[text].ToString().Substring(0, num2 * 2) + num5.ToString("x2").ToUpper().PadLeft(num3 * 2, '0') + clonedDefaultHashtable[text].ToString().Substring(num2 * 2 + num3 * 2);
						}
					}
				}
				this._UserKeyValueHash.Clear();
				foreach (DictionaryEntry item in this.DefaultKeyValueHash)
				{
					if (clonedDefaultHashtable.ContainsKey(item.Key) && item.Value.ToString() != clonedDefaultHashtable[item.Key].ToString())
					{
						this._UserKeyValueHash.Add(item.Key, clonedDefaultHashtable[item.Key]);
					}
				}
				text3 += "\r\nConfirm your change(s) ?";
				if (this._UserKeyValueHash.Count > 0)
				{
					if (MessageBox.Show(this, text3, "", MessageBoxButtons.YesNo) != DialogResult.No)
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
			this.components = new System.ComponentModel.Container();

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