//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CourseProjectADONet
{
    using System;
    using System.Collections.Generic;
    
    public partial class СтеныЭтСек2
    {
        public int ID { get; set; }
        public Nullable<int> IDСтены { get; set; }
        public Nullable<int> IDЭтажаСек2 { get; set; }
        public Nullable<int> КоличСтенНаЭтаже { get; set; }
    
        public virtual Стены Стены { get; set; }
        public virtual ЭтажиСек2 ЭтажиСек2 { get; set; }
    }
}
