﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clansty.tianlang.SIAMC
{
    public partial class Loading : Form
    {
        public Loading()
        {
            InitializeComponent();
            var siamc = new Siamc();
            Task.Run(() =>
            {
                HashSet<long> uins = new HashSet<long>();
                var members = Robot.GetGroupMembers(G.major);
                foreach (var i in members)
                {
                    uins.Add(i);
                }

                var rows = siamc.data.Rows;
                Invoke(new Action(() =>
                {
                    label3.Text = uins.Count.ToString();
                    progressBar1.Maximum = uins.Count;
                }));
                foreach (var i in uins)
                {
                    var u = new User(i);
                    Invoke(new Action(() =>
                    {
                        rows.Add(u.Uin, u.Branch, u.Enrollment, u.Junior, u.Grade, u.Nick, u.Name, u.Class, u.VerifyMsg, u.Role, "保存此行");
                        progressBar1.PerformStep();
                        label1.Text = progressBar1.Value.ToString();
                    }));
                }
                Invoke(new Action(() =>
                {
                    siamc.Show();
                    Dispose();
                }));
            });
        }
    }
}
