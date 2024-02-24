using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NTCC.NET.Core.Facility
{
    public class FacilitySet<T> where  T : FacilityElement
    {   

        public bool Add(T element)
        {
            if (Items.ContainsKey(element.ID))
                return false;

            Items.Add(element.ID, element);
            return true;
        }


        public T this[string id]
        {
            get
            {
                if (!Items.ContainsKey(id))
                    return null;

                return Items[id];
            }
            set => Items[id] = value;
        }

        public Dictionary<string, T> Items
        {
            get;
            private set;

        } = new Dictionary<string, T>();
    }
}
