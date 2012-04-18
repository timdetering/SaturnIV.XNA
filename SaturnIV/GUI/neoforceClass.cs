﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;

namespace SaturnIV
{
    class NFClass
    {
        ListBox buildListBox;
        public Window commandWindow;
        public Window leftPanelWindow;
        // Create our buttons.
        Button engageBtn, patrolBtn, holdBtn, cancelBtn;
        List<newShipStruct> activeShipList = new List<newShipStruct>();
                    
        public TomShane.Neoforce.Controls.Console consoleLogWindow;
        public void LoadCommandWindow(Manager manager, ref List<newShipStruct> aShipList)
        {
            activeShipList = aShipList;
            manager.LayoutDirectory = "Content/Layouts";
            commandWindow = (Window)Layout.Load(manager, "CommandWindow.xml");
            commandWindow.Visible = false;
            commandWindow.Resizable = false;
            commandWindow.CloseButtonVisible = false;
            commandWindow.Text = "Command Window";
            commandWindow.BackColor = Color.Blue;
            engageBtn = new Button(manager);
            engageBtn.Top = 25;
            engageBtn.Left = 25;
            engageBtn.Width = 150;
            engageBtn.Text = "Engage";
            engageBtn.Parent = commandWindow;
            patrolBtn = new Button(manager);
            patrolBtn.Top = 50;
            patrolBtn.Left = 25;
            patrolBtn.Width = 150;
            patrolBtn.Text = "Patrol";
            patrolBtn.Parent = commandWindow;
            holdBtn = new Button(manager);
            holdBtn.Top = 75;
            holdBtn.Left = 25;
            holdBtn.Width = 150;
            holdBtn.Text = "Hold Position";
            holdBtn.Parent = commandWindow;
            cancelBtn = new Button(manager);
            cancelBtn.Top = 100;
            cancelBtn.Left = 25;
            cancelBtn.Width = 150;
            cancelBtn.Text = "Cancel";
            cancelBtn.Parent = commandWindow;
            patrolBtn.Click += patrolBtn_Click;
            engageBtn.Click += engageBtn_Click;
            holdBtn.Click += holdBtn_Click;
            cancelBtn.Click += cancelBtn_Click;

            manager.Add(commandWindow);
            //manager.Add(leftPanelWindow);
            //consoleLogWindow = new TomShane.Neoforce.Controls.Console(manager);
            //manager.Add(consoleLogWindow);
        }

        #region Button Handlers

        void engageBtn_Click(object sender, EventArgs e)
        {
            foreach (newShipStruct tShip in activeShipList)
            {
                if (tShip.isSelected)
                {

                    MessageClass.messageLog.Add("" + tShip.objectAlias); 
                    tShip.currentDisposition = disposition.engaging;
                }
            }
            commandWindow.Visible = false;
        }

        void patrolBtn_Click(object sender, EventArgs e)
        { 
            foreach (newShipStruct tShip in activeShipList)
            {
                if (tShip.isSelected)
                {

                    MessageClass.messageLog.Add("" + tShip.objectAlias);
                    tShip.currentDisposition = disposition.engaging;
                }
            }
            commandWindow.Visible = false;
        }

        void holdBtn_Click(object sender, EventArgs e)
        {
            foreach (newShipStruct tShip in activeShipList)
            {
                if (tShip.isSelected)
                {

                    MessageClass.messageLog.Add("" + tShip.objectAlias);
                    tShip.currentDisposition = disposition.idle;
                }
            }
            commandWindow.Visible = false;
        }

        void cancelBtn_Click(object sender, EventArgs e)
        {           
            commandWindow.Visible = false;
        }
        #endregion

        public void LoadConsoleWindow(Manager manager)
        {
            consoleLogWindow = new TomShane.Neoforce.Controls.Console(manager);
        }

        public void constructionWindow(ref List<shipData> shipList, Manager manager)
        {
            buildListBox = new ListBox(manager);
            buildListBox.Width = 300;
            buildListBox.Height = 200;            
            buildListBox.Top = 450;
            buildListBox.Left = 100;
            buildListBox.Text = "Construction";
            //buildListBox.Visible = false;
            //buildListBox.ItemIndexChanged += NeoListBox_IndexChanged;
            foreach (shipData tShip in shipList)
                buildListBox.Items.Add(tShip.ShipClass);
            Button buildListCreateBtn = new Button(manager);
            buildListCreateBtn.Text = "Build";
            buildListCreateBtn.Top = 650;
            buildListCreateBtn.Left = 100;
            //buildListBox.Click += NeoButton_Create_OnClick;
            buildListCreateBtn.Init();

            //buildListCreateBtn.Parent = buildListBox;
            //manager.Add(buildListBox);
            //manager.Add(buildListCreateBtn);
            
        }
    }
}
