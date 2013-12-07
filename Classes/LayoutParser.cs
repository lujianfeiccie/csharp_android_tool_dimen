using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace layout_gen
{
    public class LayoutParser : BaseParser
    {
        public LayoutParser() : this("")
        {
        }
        public LayoutParser(string filepath):base(filepath)
        {
            this.config_filename1 = Application.StartupPath + "\\replace.xml";
            this.config_filename2 = Application.StartupPath + "\\ignore.xml";

            if (!File.Exists(this.config_filename1) ||
                !File.Exists(this.config_filename2)) {
                    writeXml();
            }
            loadXml();
            /*replace_logic.Add("android:layout_width","dimen");
            replace_logic.Add("android:layout_height", "dimen");
            not_to_replace_node_value.Add("fill_parent","");
            not_to_replace_node_value.Add("match_parent","");
            not_to_replace_node_value.Add("wrap_content", "");*/
            output_filename = "dimens.xml";
        }
        //载入配置
        private void loadXml() {
            DataSet ds;//用于存放配置文件信息
            //读配置文件
            ds = new DataSet();
            ds.ReadXml(config_filename1);
              //将读取到的记录加入到replace文件里
            foreach (DataRowView dr in ds.Tables[0].DefaultView)
            {
                string before=dr["before"].ToString();
                string after= dr["after"].ToString();
                replace_logic.Add(before, after);
            }
            ds.Dispose();
            ds = null;

            ds = new DataSet();
            ds.ReadXml(config_filename2);
            //将读取到的记录加入到replace文件里
            foreach (DataRowView dr in ds.Tables[0].DefaultView)
            {
                string value = dr["value"].ToString();
                not_to_replace_node_value.Add(value, "");
            }
            ds.Dispose();
            ds = null;
        }
        //写入配置
        private void writeXml()
        {
            XmlDocument doc=null;
            XmlElement root=null;
            XmlElement data=null;
            //========================写replace.xml文件==============================
            doc= new XmlDocument(); // 创建dom对象
            root = doc.CreateElement("root"); // 创建根节点album
            doc.AppendChild(root);    //  加入到xml document

          
            string[] before_str = {"android:layout_width","android:layout_height"};
            string[] after_str  = {"dimen","dimen"};
            for (int i = 0; i < before_str.Length; i++)
            {
                data = doc.CreateElement("data");  // 创建preview元素
                root.AppendChild(data);   // 添加到xml document

                XmlElement before = doc.CreateElement("before");  // 创建preview元素
                before.InnerText = before_str[i];

                XmlElement after = doc.CreateElement("after");  // 创建preview元素
                after.InnerText = after_str[i];

                data.AppendChild(before);
                data.AppendChild(after);   // 添加到xml document
            }
            root.AppendChild(data);
            doc.Save(config_filename1);

            //========================写ignore.xml文件==============================
            doc = new XmlDocument(); // 创建dom对象
            root = doc.CreateElement("root"); // 创建根节点album
            doc.AppendChild(root);    //  加入到xml document

            string[] value_str = { "fill_parent", "match_parent","wrap_content" };
            for (int i = 0; i < value_str.Length; i++)
            {
                data = doc.CreateElement("data");  // 创建preview元素
                root.AppendChild(data);   // 添加到xml document

                XmlElement value = doc.CreateElement("value");  // 创建preview元素
                value.InnerText = value_str[i];

                data.AppendChild(value); // 添加到xml document
            }
            root.AppendChild(data);
            doc.Save(config_filename2);
          
        }
        public override bool Parse()
        {
            bool result = base.Parse();
            xmlDoc.Save(filepath);
            return result;
        }

        public override bool Save()
        {
            writeFile();
            return true;
        }

        protected override void onNodeReceive(string node_name,XmlAttribute attr)
        {
            base.onNodeReceive(node_name,attr);
        //    Console.WriteLine("node receive");
            //output_string.Append("");
        }


    }
}
