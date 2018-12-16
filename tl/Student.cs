﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;

namespace tianlang
{
    public class Student
    {
        // 构造函数
        /// <summary>
        /// 创建 Student 并用 UID 填充
        /// </summary>
        /// <param name="Uid"></param>
        public Student(int Uid)
        {
            uid = Uid;
            DataSet data = Db.Query($"SELECT * FROM user_info WHERE uid={uid}");
            qq = data.Tables[0].Rows[0]["QQ"].ToString();
            name = data.Tables[0].Rows[0]["name"].ToString();
            Class = (int)data.Tables[0].Rows[0]["class"];
            branch = (bool)data.Tables[0].Rows[0]["branch"];
            nick = data.Tables[0].Rows[0]["nick"].ToString();
            junior = (bool)data.Tables[0].Rows[0]["junior"];
            enrollment = (int)data.Tables[0].Rows[0]["enrollment"];
            nameCard = IRQQApi.Api_GetGroupCard(C.w, G.major, qq);
        }
        /// <summary>
        /// 创建 Student 并用 QQ 填充
        /// </summary>
        /// <param name="QQ"></param>
        public Student(string QQ)
        {
            qq = QQ;
            DataSet data = Db.Query($"SELECT * FROM user_info WHERE QQ='{qq}'");
            uid = (int)data.Tables[0].Rows[0]["uid"];
            name = data.Tables[0].Rows[0]["name"].ToString();
            Class = (int)data.Tables[0].Rows[0]["class"];
            branch = (bool)data.Tables[0].Rows[0]["branch"];
            nick = data.Tables[0].Rows[0]["nick"].ToString();
            junior = (bool)data.Tables[0].Rows[0]["junior"];
            enrollment = (int)data.Tables[0].Rows[0]["enrollment"];
            nameCard = IRQQApi.Api_GetGroupCard(C.w, G.major, qq);

        }

        // 成员
        public int uid = 0;
        public string qq = "";
        public string name = "";
        public int Class = 0;
        public bool branch = false;
        public string nick = "";
        public readonly string nameCard = "";
        private string Grade = "";
        private int Enrollment = 0;
        public bool junior = false;
        // 封装字段
        public string grade {
            get => Grade;
            set
            {
                switch (value)
                {
                    case "高一":
                    case "高1":
                        Enrollment = 2018;
                        Grade = "高一";
                        break;
                    case "高二":
                    case "高2":
                        Enrollment = 2017;
                        Grade = "高二";
                        break;
                    case "高三":
                    case "高3":
                        Enrollment = 2016;
                        Grade = "高三";
                        break;
                    case "初一":
                    case "初1":
                        Enrollment = 2018;
                        Grade = "初一";
                        break;
                    case "初二":
                    case "初2":
                        Enrollment = 2017;
                        Grade = "初二";
                        break;
                    case "初三":
                    case "初3":
                        Enrollment = 2016;
                        Grade = "初三";
                        break;

                }
            }
        }

        public int enrollment
        {
            get => Enrollment;
            set
            {
                if (10 < value && value < 20)
                value += 2000;

                if (junior)
                    switch (value)
                    {
                        case 2018:
                            Grade = "初一";
                            break;
                        case 2017:
                            Grade = "初二";
                            break;
                        case 2016:
                            Grade = "初三";
                            break;
                    }
                else
                    switch (value)
                    {
                        case 2018:
                            Grade = "高一";
                            break;
                        case 2017:
                            Grade = "高二";
                            break;
                        case 2016:
                            Grade = "高三";
                            break;
                    }
                if (value < 2016)
                    Grade = (value + 3).ToString() + "届" + (junior ? "初中" : "");
                Enrollment = value;
            }
        }
        /// <summary>
        /// 解析，填充
        /// </summary>
        /// <param name="origin">校区年级班级</param>
        public void Fill(string origin)
        {
            if (origin.IndexOf("金阊") != -1)
            {
                branch = true;

            }
            if (origin.IndexOf("初") != -1)
                junior = true;
            try
            {
                string tmp = "";
                if (junior)
                    tmp = origin.Substring(origin.IndexOf("初"), 2);
                else
                    tmp = origin.Substring(origin.IndexOf("高"), 2);
                if (tmp != "")
                    grade = tmp;
            }
            catch
            {
                int tmp = 0;
                if (origin.IndexOf("届") != -1)
                    tmp = Convert.ToInt32(Regex.Replace(origin.GetLeft("届").Trim(), @"[^0-9]+", "").Trim());
                if (tmp > 0)
                    enrollment = tmp - 3;
            }


        }
    }
}
