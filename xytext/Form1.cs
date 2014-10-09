﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;

namespace xytext
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.DragEnter += new DragEventHandler(file_DragEnter);
            this.DragDrop += new DragEventHandler(file_DragDrop);
            InitializeComponent();
            
            setStringsDataGridView(new string[] { "Load a folder containing the text files.", "Text lines will then be displayed.", "Use the ComboBox / Dropdown menu to select what entry to display." });
        }
        public string[] files;
        // IO
        private void dumpTXT_Click(object sender, EventArgs e)
        {
            if (files.Length > 0)
            {
                SaveFileDialog saveDump = new SaveFileDialog();
                saveDump.Filter = "Text File|*.txt";
                DialogResult sdr = saveDump.ShowDialog();
                if (sdr == DialogResult.OK)
                {
                    bool newline = false;
                    if (MessageBox.Show("Remove newline formatting?", "Alert", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        newline = true;
                    string path = saveDump.FileName;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (TextWriter tw = new StreamWriter(ms))
                            for (int i = 0; i < files.Length; i++)
                            {
                                string[] data = getStringsFromFile(files[i]);
                                tw.WriteLine("~~~~~~~~~~~~~~~");
                                tw.WriteLine("Text File : " + i.ToString());
                                tw.WriteLine("~~~~~~~~~~~~~~~");
                                if (data != null)
                                    foreach (string line in data)
                                    {
                                        if (newline) 
                                            tw.WriteLine(line.Replace("\\n\\n"," ").Replace("\\n", " ").Replace("\\c", "").Replace("\\r", "")); // Strip out formatting
                                        else
                                            tw.WriteLine(line);
                                    }
                            }
                        File.WriteAllBytes(path, ms.ToArray());
                    }
                }
            }
        }
        private void file_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void file_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string path = files[0]; // open first D&D
            if (Directory.Exists(path))
                openFolderPath(path);
        }
        private void changeEntry(object sender, EventArgs e)
        {
            string file = files[CB_Entry.SelectedIndex];
            string[] data = getStringsFromFile(file);
            setStringsDataGridView(data);
        }
        private void B_SaveText_Click(object sender, EventArgs e)
        {
            File.WriteAllBytes(files[CB_Entry.SelectedIndex], getBytesForFile(getCurrentDGLines()));
        }
        private void openFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
                openFolderPath(fbd.SelectedPath);
        }
        private void openFolderPath(string path)
        {
            files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                TB_Path.Text = "";
                return;
            }
            TB_Path.Text = path;
            CB_Entry.Items.Clear();

            // Add all the valid entries.
            for (int i = 0; i < files.Length; i++)
                CB_Entry.Items.Add(i.ToString());

            // Enable Text Line Editing Interface
            CB_Entry.Enabled = true;
            B_SaveText.Enabled = true;
            B_AddLine.Enabled = B_RemoveLine.Enabled = true;
            CB_Entry.SelectedIndex = 0;

            // Enable Dumping All Text
            menu_Tools.Enabled = true;
        }
                
        // Top Level Functions
        private string[] getStringsFromFile(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            ushort textSections = BitConverter.ToUInt16(data, 0);
            ushort lineCount = BitConverter.ToUInt16(data, 2);
            if (lineCount == 0) return null;
            uint totalLength = BitConverter.ToUInt32(data, 4);
            uint initialKey = BitConverter.ToUInt32(data, 8);
            int sectionData = BitConverter.ToInt32(data, 12);

            try // Some sanity checking to prevent errors.
            {
            if (initialKey != 0) throw new Exception("Invalid initial key! Not 0?");
            if (sectionData + totalLength != data.Length || textSections != 1) throw new Exception("Invalid Text File");

            uint sectionLength = BitConverter.ToUInt32(data, sectionData);
            if (sectionLength != totalLength) throw new Exception("Section size and overall size do not match.");
            }
            catch { return null; };

            // Prep result storage.
            ushort key = 0x7C89;
            string[] result = new string[lineCount];

            for (int i = 0; i < lineCount; i++)
            {
                // Init
                ushort k = key;
                string s = "";
                int offset = BitConverter.ToInt32(data, i * 8 + sectionData + 4) + sectionData;
                int length = BitConverter.ToInt16(data, i * 8 + sectionData + 8);
                int start = offset;
                ushort c = 0; // u16 char

                while (offset < start + length * 2) // loop through the entire text line
                {
                    decryptU16(data, ref offset, ref c, ref k);
                    if (c == 0)             // Terminated Line
                        break;
                    else if (c == '\n') s += "\\n";
                    else if (c == 0x10)     // Variable
                        decryptVar(data, ref offset, ref s, ref c, ref k);
                    else                    // Regular Character
                    {
                        // Check special characters...
                        if (c == 0xE07F)
                            s += (char)0x202F; // nbsp
                        else if (c == 0xE08D)
                            s += (char)0x2026; // …
                        else if (c == 0xE08E)
                            s += (char)0x2642; // ♂
                        else if (c == 0xE08F)
                            s += (char)0x2640; // ♀
                        // Else append character without modification.
                        else s += (char)c;
                    }
                }
                // store string and set key for next line (if needed)
                result[i] = s;
                key += 0x2983;
            }
            return result;
        }
        private byte[] getBytesForFile(string[] lines)
        {

            using (MemoryStream textFile = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(textFile))
            using (MemoryStream data = new MemoryStream())
            using (BinaryWriter bz = new BinaryWriter(data))
            {
                ushort baseKey = 0x7C89;

                // Write up header template
                bw.Write((ushort)1);            // Always 1 ? 
                bw.Write((ushort)lines.Length); // Line Count
                bw.Write((uint)0);              // (Temporary) Data Length - fixed at the end.
                bw.Write((uint)0);              // Key, constant 0.
                bw.Write((uint)0x10);           // Pointer to line data.

                // Begin data
                bw.Write((uint)0);              // (Temporary) Data Length - fixed at the end.

                for (int i = 0; i < lines.Length; i++)
                {
                    ushort key = baseKey;
                    uint pos = (uint)data.Position;
                    // Get crypted line data.
                    {
                        {
                            // Write each character to the data stream, with handling for special characters.
                            for (int j = 0; j < lines[i].Length; j++)
                            {
                                ushort val = lines[i][j];

                                // Handle special text characters
                                // Private Use Area characters
                                if (val == 0x202F) val = 0xE07F;        // nbsp
                                else if (val == 0x2026) val = 0xE08D;   // …
                                else if (val == 0x2642) val = 0xE08E;   // ♂
                                else if (val == 0x2640) val = 0xE08F;   // ♀

                                // Variables
                                else if (val == '[' || val == '\\')          // Variable
                                    encryptVar(bz, lines[i], ref j, ref key);

                                // Text
                                else bz.Write(encryptU16(val, ref key));
                            }
                            bz.Write(encryptU16(0, ref key)); // Add the null terminator, after encrypting it.
                        }

                        // Write the lineOffset and charCount to the header.
                        bw.Write((uint)(pos + 0x4 + lines.Length * 8));
                        bw.Write((uint)(data.Position - pos) / 2);
                        if (data.Position % 4 > 0) bz.Write((ushort)0);

                        // Increment the line initial key for the next line.
                        baseKey += 0x2983;
                    }
                }

                // Copy the data stream to the textFile stream.
                data.Position = 0; data.CopyTo(textFile);

                // Fix the temp values
                textFile.Position = 0x4; bw.Write((uint)textFile.Length - 0x10);
                textFile.Position = 0x10; bw.Write((uint)textFile.Length - 0x10);

                return textFile.ToArray();
            }
        }
        // Main Handling
        private void setStringsDataGridView(string[] textArray)
        {
            // Clear the datagrid row content to remove all text lines.
            dgv.Rows.Clear();

            if (textArray == null) // Error handling for bad inputs.
                return;

            // Clear the header columns, these are repopulated every time.
            dgv.Columns.Clear();

            // Reset settings and columns.
            dgv.AllowUserToResizeColumns = false;
            DataGridViewColumn dgvLine = new DataGridViewTextBoxColumn();
            {
                dgvLine.HeaderText = "Line";
                dgvLine.DisplayIndex = 0;
                dgvLine.Width = 32;
                dgvLine.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvLine.ReadOnly = true;
                dgvLine.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            DataGridViewTextBoxColumn dgvText = new DataGridViewTextBoxColumn();
            {
                dgvText.HeaderText = "Text";
                dgvText.DisplayIndex = 1;
                dgvText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvText.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // Re-add the columns.
            dgv.Columns.Add(dgvLine);
            dgv.Columns.Add(dgvText);

            // Add empty rows equal to how many entries.
            dgv.Rows.Add(textArray.Length);

            // Add the text lines into their cells.
            for (int i = 0; i < textArray.Length; i++)
            {
                dgv.Rows[i].Cells[0].Value = i;
                dgv.Rows[i].Cells[1].Value = textArray[i];
            }
        }
        private string[] getCurrentDGLines()
        {
            // Get Line Count
            string[] lines = new string[dgv.RowCount];
            for (int i = 0; i < dgv.RowCount; i++)
                lines[i] = (string)dgv.Rows[i].Cells[1].Value;
            return lines;
        }
        // Meta Usage
        private void B_AddLine_Click(object sender, EventArgs e)
        {
            int currentRow = 0;
            try
            {
                currentRow = dgv.CurrentRow.Index;
            }
            catch
            {
                dgv.Rows.Add();
            }

            if (currentRow < dgv.Rows.Count - 1 || currentRow == 0)
            {
                if (ModifierKeys != Keys.Control && currentRow != 0)
                {
                    DialogResult dr = MessageBox.Show("Inserting in between rows will shift all subsequent lines.\n\nContinue?", "Alert", MessageBoxButtons.YesNo);
                    if (dr != DialogResult.Yes) return;
                }
                dgv.Rows.Insert(currentRow + 1);
            }

            for (int i = 0; i < dgv.Rows.Count; i++)
                dgv.Rows[i].Cells[0].Value = i.ToString();
        }
        private void B_RemoveLine_Click(object sender, EventArgs e)
        {
            int currentRow = dgv.CurrentRow.Index;
            if (currentRow < dgv.Rows.Count - 1)
            {
                if (ModifierKeys != Keys.Control)
                {
                    DialogResult dr = MessageBox.Show("Deleting a row above other lines will shift all subsequent lines.\n\nContinue?", "Alert", MessageBoxButtons.YesNo);
                    if (dr != DialogResult.Yes) return;
                }
            }

            dgv.Rows.RemoveAt(currentRow);

            for (int i = 0; i < dgv.Rows.Count; i++)
                dgv.Rows[i].Cells[0].Value = i.ToString();
        }
        // Text Encrypting
        private ushort encryptU16(ushort val, ref ushort key)
        {
            val = (ushort)(key ^ val);
            key = (ushort)(((key << 3) | (key >> 13)) & 0xffff);
            return val;
        }
        private ushort decryptU16(byte[] data, ref int offset, ref ushort val, ref ushort key)
        {
            val = (ushort)(BitConverter.ToUInt16(data, offset) ^ key); 
            offset += 2;
            key = (ushort)(((key << 3) | (key >> 13)) & 0xffff);
            return val;
        }
        // Variable Handling
        private ushort getVariableBytes(string varType, ref List<ushort> args)
        {
            // Fetch the variable name...
            int bracket = varType.IndexOf('(');
            string variable = varType.Substring(0, bracket);
            string[] arguments = varType.Substring(bracket + 1, varType.Length - bracket - 2).Split(',');

            ushort varVal = 0;

            switch (variable)
            {
                case "COLOR": varVal = 0xFF00; break;
                case "TRNAME": varVal = 0x0100; break;
                case "PKNAME": varVal = 0x0101; break;
                case "PKNICK": varVal = 0x0102; break;
                case "TYPE": varVal = 0x0103; break;
                case "LOCATION": varVal = 0x0105; break;
                case "ABILITY": varVal = 0x0106; break;
                case "MOVE": varVal = 0x0107; break;
                case "ITEM1": varVal = 0x0108; break;
                case "ITEM2": varVal = 0x0109; break;
                case "sTRBAG": varVal = 0x010A; break;
                case "BOX": varVal = 0x010B; break;
                case "EVSTAT": varVal = 0x010D; break;
                case "OPOWER": varVal = 0x0110; break;
                case "RIBBON": varVal = 0x0127; break;
                case "MIINAME": varVal = 0x0134; break;
                case "WEATHER": varVal = 0x013E; break;
                case "TRNICK": varVal = 0x0189; break;
                case "1stchrTR": varVal = 0x018A; break;
                case "SHOUTOUT": varVal = 0x018B; break;
                case "BERRY": varVal = 0x018E; break;
                case "REMFEEL": varVal = 0x018F; break;
                case "REMQUAL": varVal = 0x0190; break;
                case "WEBSITE": varVal = 0x0191; break;
                case "CHOICECOS": varVal = 0x019C; break;
                case "GSYNCID": varVal = 0x01A1; break;
                case "PRVIDSAY": varVal = 0x0192; break;
                case "BTLTEST": varVal = 0x0193; break;
                case "GENLOC": varVal = 0x0195; break;
                case "CHOICEFOOD": varVal = 0x0199; break;
                case "HOTELITEM": varVal = 0x019A; break;
                case "TAXISTOP": varVal = 0x019B; break;
                case "MAISTITLE": varVal = 0x019F; break;
                case "ITEMPLUR0": varVal = 0x1000; break;
                case "ITEMPLUR1": varVal = 0x1001; break;
                case "GENDBR": varVal = 0x1100; break;
                case "NUMBRNCH": varVal = 0x1101; break;
                case "iCOLOR2": varVal = 0x1302; break;
                case "iCOLOR3": varVal = 0x1303; break;
                case "NUM1": varVal = 0x0200; break;
                case "NUM2": varVal = 0x0201; break;
                case "NUM3": varVal = 0x0202; break;
                case "NUM4": varVal = 0x0203; break;
                case "NUM5": varVal = 0x0204; break;
                case "NUM6": varVal = 0x0205; break;
                case "NUM7": varVal = 0x0206; break;
                case "NUM8": varVal = 0x0207; break;
                case "NUM9": varVal = 0x0208; break;
                default: varVal = Convert.ToUInt16(variable, 16); break;
            }
            // Set arguments in.
            for (int i = 0; i < arguments.Length; i++)
                args.Add(Convert.ToUInt16(arguments[i], 16));

            // All done.
            return varVal;
        }
        private void encryptVar(BinaryWriter bw, string line, ref int i, ref ushort key)
        {
            ushort val = line[i];
            if (val == '\\')        // Line Break
                if (line[i + 1] == 'n')
                {
                    i++;
                    bw.Write(encryptU16('\n', ref key));
                }
                else if (line[i + 1] == 'r')
                {
                    bw.Write(encryptU16(0x10, ref key)); i++;
                    bw.Write(encryptU16(1, ref key)); 
                    bw.Write(encryptU16(0xBE00, ref key)); 
                }
                else if (line[i + 1] == 'c')
                {
                    bw.Write(encryptU16(0x10, ref key)); i++;
                    bw.Write(encryptU16(1, ref key)); 
                    bw.Write(encryptU16(0xBE01, ref key)); 
                }
                else { throw new Exception("Invalid terminated line"); }
            else if (val == '[')    // Special Variable
            {
                int bracket = line.Substring(i + 1).IndexOf(']');
                if (bracket < 3) throw new Exception("Variable encoding error!");

                // [VAR X(a, b)]
                // Remove the [ ] -> VAR X(a, b)
                string varCMD = line.Substring(i + 1, bracket);
                i += bracket + 1; // Advance the index to the end of the bracket.

                string varMethod = varCMD.Split(' ')[0];                    // Returns VAR or WAIT or ~
                string varType = varCMD.Substring(varMethod.Length + 1);    // Returns the remainder of the var command data.
                ushort varValue = 0;

                // Set up argument storage (even if it not used)
                List<ushort> args = new List<ushort>();

                try
                {
                    switch (varMethod)
                    {
                        case "~":       // Blank Text Line Variable (No text set - debug/quality testing variable?)
                            {
                                varValue = 0xBDFF;
                                args.Add(Convert.ToUInt16(varType));
                                break;
                            }
                        case "WAIT":    // Event pause Variable.
                            {
                                varValue = 0xBE02;
                                args.Add(Convert.ToUInt16(varType));
                                break;
                            }
                        case "VAR":     // Text Variable
                            {
                                varValue = getVariableBytes(varType, ref args);
                                break;
                            }
                        default: throw new Exception("Unknown variable method type!");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Variable error. Current line text is:\n\n" + line + "\n\n" + e.ToString(), "Alert");
                }

                // Write the Variable prefix.
                bw.Write(encryptU16(0x0010, ref key));
                // Write Length of the following Variable Data
                bw.Write(encryptU16((ushort)(1 + args.Count), ref key));
                // Write the Variable type.
                bw.Write(encryptU16(varValue, ref key));

                for (int j = 0; j < args.Count; j++)
                    bw.Write(encryptU16((ushort)args[j], ref key));

                // Done.
            }
        }
        private void decryptVar(byte[] d, ref int o, ref string s, ref ushort v, ref ushort k)
        {
            ushort length = decryptU16(d, ref o, ref v, ref k);
            ushort varType = decryptU16(d, ref o, ref v, ref k);
            switch (varType)
            {
                // Check the nonvariable types...
                case 0xBE00: // "Waitbutton then scroll text;; \r"
                    { s += "\\r"; return; }
                case 0xBE01: // "Waitbutton then clear text;; \c"
                    { s += "\\c"; return; }
                case 0xBE02: // Dramatic pause for a text line. New!
                    { s += "[WAIT " + decryptU16(d, ref o, ref v, ref k).ToString() + "]"; return; }
                case 0xBDFF: // Empty Text line? Includes linenum so maybe for betatest finding used-unused lines?
                    { s += "[~ " + decryptU16(d, ref o, ref v, ref k).ToString() + "]"; return; }

                // Else a text variable, so let's loop through all the variable types. If we cannot find it, we just write the u16 val.
                default:
                    {
                        ushort varCode = varType; // decryptU16(d, ref o, ref v, ref k);
                        string tvname = "";
                        switch (varCode) // get variable's info name
                        {
                            case 0xFF00: tvname = "COLOR"; break; // Change text line color (0 = white, 1 = red, 2 = blue...)
                            case 0x0100: tvname = "TRNAME"; break; // 
                            case 0x0101: tvname = "PKNAME"; break;
                            case 0x0102: tvname = "PKNICK"; break;
                            case 0x0103: tvname = "TYPE"; break;
                            case 0x0105: tvname = "LOCATION"; break;
                            case 0x0106: tvname = "ABILITY"; break;
                            case 0x0107: tvname = "MOVE"; break;
                            case 0x0108: tvname = "ITEM1"; break;
                            case 0x0109: tvname = "ITEM2"; break;
                            case 0x010A: tvname = "sTRBAG"; break;
                            case 0x010B: tvname = "BOX"; break;
                            case 0x010D: tvname = "EVSTAT"; break;
                            case 0x0110: tvname = "OPOWER"; break;
                            case 0x0127: tvname = "RIBBON"; break;
                            case 0x0134: tvname = "MIINAME"; break;
                            case 0x013E: tvname = "WEATHER"; break;
                            case 0x0189: tvname = "TRNICK"; break;
                            case 0x018A: tvname = "1stchrTR"; break;
                            case 0x018B: tvname = "SHOUTOUT"; break;
                            case 0x018E: tvname = "BERRY"; break;
                            case 0x018F: tvname = "REMFEEL"; break;
                            case 0x0190: tvname = "REMQUAL"; break;
                            case 0x0191: tvname = "WEBSITE"; break;
                            case 0x019C: tvname = "CHOICECOS"; break;
                            case 0x01A1: tvname = "GSYNCID"; break;
                            case 0x0192: tvname = "PRVIDSAY"; break;
                            case 0x0193: tvname = "BTLTEST"; break;
                            case 0x0195: tvname = "GENLOC"; break;
                            case 0x0199: tvname = "CHOICEFOOD"; break;
                            case 0x019A: tvname = "HOTELITEM"; break;
                            case 0x019B: tvname = "TAXISTOP"; break;
                            case 0x019F: tvname = "MAISTITLE"; break;
                            case 0x1000: tvname = "ITEMPLUR0"; break;
                            case 0x1001: tvname = "ITEMPLUR1"; break;
                            case 0x1100: tvname = "GENDBR"; break;
                            case 0x1101: tvname = "NUMBRNCH"; break;
                            case 0x1302: tvname = "iCOLOR2"; break;
                            case 0x1303: tvname = "iCOLOR3"; break;
                            case 0x0200: tvname = "NUM1"; break;
                            case 0x0201: tvname = "NUM2"; break;
                            case 0x0202: tvname = "NUM3"; break;
                            case 0x0203: tvname = "NUM4"; break;
                            case 0x0204: tvname = "NUM5"; break;
                            case 0x0205: tvname = "NUM6"; break;
                            case 0x0206: tvname = "NUM7"; break;
                            case 0x0207: tvname = "NUM8"; break;
                            case 0x0208: tvname = "NUM9"; break;
                            default: tvname = varCode.ToString("X4");  break;
                        }
                        s += "[VAR" + " " + tvname;
                        if (length > 1)
                        {
                            s += '(';
                            while (length > 1)
                            {
                                // Write arguments
                                ushort arg = decryptU16(d, ref o, ref v, ref k);
                                length--;
                                s += arg.ToString("X4");
                                if (length == 1) break;
                                s += ",";
                            }
                            s += ')';
                        }                        
                        s += "]";
                        break;
                    }
            }
        }
    }
}
