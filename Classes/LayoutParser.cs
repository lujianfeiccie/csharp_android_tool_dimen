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
        //��������
        private void loadXml() {
            DataSet ds;//���ڴ�������ļ���Ϣ
            //�������ļ�
            ds = new DataSet();
            ds.ReadXml(config_filename1);
              //����ȡ���ļ�¼���뵽replace�ļ���
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
            //����ȡ���ļ�¼���뵽replace�ļ���
            foreach (DataRowView dr in ds.Tables[0].DefaultView)
            {
                string value = dr["value"].ToString();
                not_to_replace_node_value.Add(value, "");
            }
            ds.Dispose();
            ds = null;
        }
        //д������
        private void writeXml()
        {
            XmlDocument doc=null;
            XmlElement root=null;
            XmlElement data=null;
            //========================дreplace.xml�ļ�==============================
            doc= new XmlDocument(); // ����dom����
            root = doc.CreateElement("root"); // �������ڵ�album
            doc.AppendChild(root);    //  ���뵽xml document

          
            string[] before_str = {"android:layout_width","android:layout_height"};
            string[] after_str  = {"dimen","dimen"};
            for (int i = 0; i < before_str.Length; i++)
            {
                data = doc.CreateElement("data");  // ����previewԪ��
                root.AppendChild(data);   // ��ӵ�xml document

                XmlElement before = doc.CreateElement("before");  // ����previewԪ��
                before.InnerText = before_str[i];

                XmlElement after = doc.CreateElement("after");  // ����previewԪ��
                after.InnerText = after_str[i];

                data.AppendChild(before);
                data.AppendChild(after);   // ��ӵ�xml document
            }
            root.AppendChild(data);
            doc.Save(config_filename1);

            //========================дignore.xml�ļ�==============================
            doc = new XmlDocument(); // ����dom����
            root = doc.CreateElement("root"); // �������ڵ�album
            doc.AppendChild(root);    //  ���뵽xml document

            string[] value_str = { "fill_parent", "match_parent","wrap_content" };
            for (int i = 0; i < value_str.Length; i++)
            {
                data = doc.CreateElement("data");  // ����previewԪ��
                root.AppendChild(data);   // ��ӵ�xml document

                XmlElement value = doc.CreateElement("value");  // ����previewԪ��
                value.InnerText = value_str[i];

                data.AppendChild(value); // ��ӵ�xml document
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
