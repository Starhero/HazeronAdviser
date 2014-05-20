﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace HazeronAdviser
{
    public partial class Main : Form
    {
        List<HCityObj> hCityList = new List<HCityObj>();
        List<HShipObj> hShipList = new List<HShipObj>();
        List<HOfficerObj> hOfficerList = new List<HOfficerObj>();

        Image imageCity;
        Image imageShip;
        Image imageOfficer;

        public Main()
        {
            InitializeComponent();
            toolStripProgressBar1.Visible = false;
            imageCity = HazeronAdviser.Properties.Resources.c_Flag;
            imageShip = HazeronAdviser.Properties.Resources.GovSpacecraft;
            imageOfficer = HazeronAdviser.Properties.Resources.Officer;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar2.Visible = false;
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Shores of Hazeron", "Mail")))
            {
                toolStripStatusLabel1.Text = "\""+Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Shores of Hazeron", "Mail")+"\" does not exist.";
                if (DialogResult.Yes == MessageBox.Show("Could not find Hazeron Mail folder:" + Environment.NewLine + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Shores of Hazeron", "Mail") + Environment.NewLine + Environment.NewLine + "Copy directory path to clipboard?", "Mail Folder Not Found", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2))
                    Clipboard.SetText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Shores of Hazeron", "Mail"));
                return;
            }
            string[] fileList = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Shores of Hazeron", "Mail")); // %USERPROFILE%\Shores of Hazeron\Mail
            toolStripStatusLabel1.Text = "Scanning mails...";
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = fileList.Length;
            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel1.Invalidate();
            statusStrip1.Update();
            dgvCity.Rows.Clear();
            dgvShip.Rows.Clear();
            dgvOfficer.Rows.Clear();
            dgvCity.Refresh();
            dgvShip.Refresh();
            dgvOfficer.Refresh();
            tbxCity.Clear();
            tbxShip.Clear();
            tbxOfficer.Clear();
            //textBox2.Text = String.Join(Environment.NewLine, fileList); // Lists all files in the SoH mail folder.
            foreach (string file in fileList)
            {
                try
                {
                    if (HMail.IsCityReport(file))
                    {
                        HCityObj temp = new HCityObj(new HMailObj(file));
                        if (hCityList.Any(city => city.ID == temp.ID))
                            hCityList[hCityList.FindIndex(city => city.ID == temp.ID)].Update(new HMailObj(file));
                        else
                            hCityList.Add(temp);
                    }
                    else if (HMail.IsShipLog(file))
                    {
                        HShipObj temp = new HShipObj(new HMailObj(file));
                        if (hShipList.Any(ship => ship.ID == temp.ID))
                            hShipList[hShipList.FindIndex(ship => ship.ID == temp.ID)].Update(new HMailObj(file));
                        else
                            hShipList.Add(temp);
                    }
                    else if (HMail.IsOfficerTenFour(file))
                    {
                        HOfficerObj temp = new HOfficerObj(new HMailObj(file));
                        if (hOfficerList.Any(officer => officer.ID == temp.ID))
                            hOfficerList[hOfficerList.FindIndex(ship => ship.ID == temp.ID)].Update(new HMailObj(file));
                        else
                            hOfficerList.Add(temp);
                    }
                    toolStripProgressBar1.Increment(1);
                }
                catch (Exception ex)
                {
                    toolStripStatusLabel1.Text = "Error while scanning mail file: " + file;
                    if (DialogResult.Yes == MessageBox.Show("Failed reading mail file:" + Environment.NewLine + file + Environment.NewLine + Environment.NewLine + "Copy mail filepath to clipboard?", "Mail Reading Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2))
                        Clipboard.SetText(file);
                    return;
                }
            }
            toolStripProgressBar2.Value = 0;
            toolStripProgressBar2.Maximum = hCityList.Count + hShipList.Count + hOfficerList.Count;
            toolStripProgressBar2.Visible = true;
            toolStripStatusLabel1.Text = "Filling tables...";
            toolStripStatusLabel1.Invalidate();
            statusStrip1.Update();
            foreach (var hCity in hCityList)
            {
                dgvCity.Rows.Add();
                dgvCity.Rows[dgvCity.RowCount - 1].Cells["ColumnCitySelection"].Value = false;
                dgvCity.Rows[dgvCity.RowCount - 1].Cells["ColumnCityIcon"].Value = imageCity;
                dgvCity.Rows[dgvCity.RowCount - 1].Cells["ColumnCityName"].Value = hCity.Name;
                dgvCity.Rows[dgvCity.RowCount - 1].Cells["ColumnCityMorale"].Value = hCity.MoraleShort;
                dgvCity.Rows[dgvCity.RowCount - 1].Cells["ColumnCityPopulation"].Value = hCity.PopulationShort;
                dgvCity.Rows[dgvCity.RowCount - 1].Cells["ColumnCityLivingConditions"].Value = hCity.LivingShort;
                dgvCity.Rows[dgvCity.RowCount - 1].Cells["ColumnCityDate"].Value = hCity.LastUpdaredString;
                if (hCity.AttentionCode != 0x00)
                {
                    dgvCity.Rows[dgvCity.RowCount - 1].Cells["ColumnCityName"].Style.BackColor = Color.LightPink;
                    if ((hCity.AttentionCode & 0x01) == 0x01)
                        dgvCity.Rows[dgvCity.RowCount - 1].Cells["ColumnCityLivingConditions"].Style.BackColor = Color.LightPink;
                    if ((hCity.AttentionCode & 0x02) == 0x02)
                        dgvCity.Rows[dgvCity.RowCount - 1].Cells["ColumnCityPopulation"].Style.BackColor = Color.LightPink;
                    if ((hCity.AttentionCode & 0x04) == 0x04)
                        dgvCity.Rows[dgvCity.RowCount - 1].Cells["ColumnCityMorale"].Style.BackColor = Color.LightPink;
                }
                toolStripProgressBar2.Increment(1);
            }
            foreach (var hShip in hShipList)
            {
                dgvShip.Rows.Add();
                dgvShip.Rows[dgvShip.RowCount - 1].Cells["ColumnShipSelection"].Value = false;
                dgvShip.Rows[dgvShip.RowCount - 1].Cells["ColumnShipIcon"].Value = imageShip;
                dgvShip.Rows[dgvShip.RowCount - 1].Cells["ColumnShipName"].Value = hShip.Name;
                dgvShip.Rows[dgvShip.RowCount - 1].Cells["ColumnShipFuel"].Value = hShip.FuelShort;
                dgvShip.Rows[dgvShip.RowCount - 1].Cells["ColumnShipDamage"].Value = hShip.DamageShort;
                dgvShip.Rows[dgvShip.RowCount - 1].Cells["ColumnShipDate"].Value = hShip.LastUpdaredString;
                toolStripProgressBar2.Increment(1);
            }
            foreach (var hOfficer in hOfficerList)
            {
                dgvOfficer.Rows.Add();
                dgvOfficer.Rows[dgvOfficer.RowCount - 1].Cells["ColumnOfficerSelection"].Value = false;
                dgvOfficer.Rows[dgvOfficer.RowCount - 1].Cells["ColumnOfficerIcon"].Value = imageOfficer;
                dgvOfficer.Rows[dgvOfficer.RowCount - 1].Cells["ColumnOfficerName"].Value = hOfficer.Name;
                dgvOfficer.Rows[dgvOfficer.RowCount - 1].Cells["ColumnOfficerHome"].Value = hOfficer.Home;
                dgvOfficer.Rows[dgvOfficer.RowCount - 1].Cells["ColumnOfficerLocation"].Value = hOfficer.Location;
                dgvOfficer.Rows[dgvOfficer.RowCount - 1].Cells["ColumnOfficerDate"].Value = hOfficer.LastUpdaredString;
                toolStripProgressBar2.Increment(1);
            }
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar2.Visible = false;
            toolStripStatusLabel1.Text = "Done!";
        }

        private void dgvCity_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCity.SelectedRows.Count != 0 && dgvCity.SelectedRows[0].Index != -1)
            {
                tbxCity.Text = hCityList[dgvCity.SelectedRows[0].Index].BodyTest;
            }
        }

        private void dgvShip_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvShip.SelectedRows.Count != 0 && dgvShip.SelectedRows[0].Index != -1)
            {
                tbxShip.Text = hShipList[dgvShip.SelectedRows[0].Index].BodyTest;
            }
        }

        private void dgvOfficer_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvOfficer.SelectedRows.Count != 0 && dgvOfficer.SelectedRows[0].Index != -1)
            {
                tbxOfficer.Text = hOfficerList[dgvOfficer.SelectedRows[0].Index].BodyTest;
            }
        }
    }
}