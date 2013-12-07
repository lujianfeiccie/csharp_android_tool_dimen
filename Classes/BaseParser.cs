using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO;

namespace layout_gen
{
    public class BaseParser
    {
        protected XmlDocument xmlDoc = new XmlDocument();
        protected string filepath;
        protected string filename;
        protected string output_dir;
        protected string output_filename;
        protected StringBuilder output_string = new StringBuilder();
        protected Hashtable replace_logic = new Hashtable();
        protected Hashtable not_to_replace_node_value = new Hashtable();
        protected int count_controls = 0;
        protected string config_filename1;
        protected string config_filename2;
        public BaseParser(string filepath) {
            setFilePath(filepath);
        }

        public void setFilePath(string filepath){
            if (!filepath.Equals(""))
            {
                this.filepath = filepath;
                int startindex = filepath.LastIndexOf(@"\") + 1;
                this.filename = filepath.Substring(startindex, filepath.Length - startindex);
                startindex = filename.LastIndexOf(@".");
                this.filename = filename.Substring(0, startindex);
            }
        }
        public virtual bool Parse() {
            count_controls = 0;
            output_string.Append(string.Format("\n\n<!-- {0}.xml -->",filename));
            output_string.Append("\n");
            xmlDoc.Load(filepath);
            XmlNode root = xmlDoc.DocumentElement;
            visit(root);
            VisitChildNode(root);
            return true;
        }
        public virtual bool Save() { return false; }

        public void VisitChildNode(XmlNode node)
        {
            if (node.ChildNodes.Count == 0)
            {
                return;
            }
            else
            {
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    XmlNode temp_node = node.ChildNodes[i];
                    visit(temp_node);
                    VisitChildNode(node.ChildNodes[i]);
                }
            }
        }

        protected void visit(XmlNode temp_node)
        {
            Console.WriteLine("==============================================");
            for (int j = 0; j < temp_node.Attributes.Count; j++)
            {
                Console.WriteLine("{0}_{1}={2}", temp_node.Name, temp_node.Attributes[j].Name, temp_node.Attributes[j].Value);
                //temp_node.Attributes[j].Value = "1dp";
                XmlAttribute attr = temp_node.Attributes[j];
                onNodeReceive(temp_node.Name,attr);    
            }
            
        }

        protected virtual void onNodeReceive(string node_name, XmlAttribute attr)
        {
            Console.WriteLine(attr.Name);
            if (replace_logic.ContainsKey(attr.Name) && 
                !not_to_replace_node_value.ContainsKey(attr.Value) &&
                !attr.Value[0].Equals('@'))  //包含规则中的属性名, 且属性值不于系统内置值, 且不以@开头
                
            {
                string temp_attr_name = attr.Name.Split(':')[1];
                ++count_controls;//统计需要适配属性的个数
                string dimen_name = string.Format("{0}_{1}_{2}_{3}", filename,
                                                                        node_name.ToLower(),
                                                                        temp_attr_name,
                                                                        count_controls);

                output_string.Append(string.Format("<{0} name=\"{1}\">{2}</{0}>", replace_logic[attr.Name].ToString().ToLower(),
                                                                                       dimen_name,
                                                                                        attr.Value));
                output_string.Append("\n");

                attr.Value = string.Format("@{0}/{1}", replace_logic[attr.Name], dimen_name);
            }
        }

        public void setOutputPath(string output_dir)
        {
            if(!output_dir[output_dir.Length-1].Equals('\\')){
                output_dir += "\\";
            }
            this.output_dir = output_dir;
        }
        protected void writeFile()
        {
            string tempstr = output_dir + output_filename;
            FileStream fs = null;
            StreamWriter sw = null;
            
            fs = new FileStream(tempstr, FileMode.Create);
            sw = new StreamWriter(fs, Encoding.Default);
            
            sw.Write("<resources>\n" + getOutput_string() + "\n</resources>");
            
            sw.Close();
            fs.Close();
        }
        public string getOutput_string()
        {
            return output_string.ToString();
        }
    }
}
