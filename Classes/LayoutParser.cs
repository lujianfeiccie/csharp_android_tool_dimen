using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace layout_gen
{
    public class LayoutParser : BaseParser
    {
        
        public LayoutParser(string filepath):base(filepath)
        {
            replace_logic.Add("android:layout_width","dimen");
            replace_logic.Add("android:layout_height", "dimen");
            not_to_replace_node_value.Add("fill_parent","");
            not_to_replace_node_value.Add("match_parent","");
            not_to_replace_node_value.Add("wrap_content", "");
            output_filename = "dimens.xml";
        }
        public override bool Parse()
        {
            return base.Parse();
        }

        public override bool Save()
        {
            xmlDoc.Save(filepath);
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
