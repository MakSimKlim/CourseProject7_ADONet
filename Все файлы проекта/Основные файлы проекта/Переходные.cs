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
    
    public partial class Переходные
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Переходные()
        {
            this.ПерехБет = new HashSet<ПерехБет>();
            this.ПерехЭтСек1 = new HashSet<ПерехЭтСек1>();
            this.ПерехЭтСек2 = new HashSet<ПерехЭтСек2>();
        }
    
        public int IDПереходной { get; set; }
        public string Наименование { get; set; }
        public Nullable<decimal> Объем { get; set; }
        public Nullable<decimal> Толщина { get; set; }
        public string ЛистПроекта { get; set; }
        public Nullable<int> IDПроекта { get; set; }
        public byte[] Изображение { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ПерехБет> ПерехБет { get; set; }
        public virtual Проекты Проекты { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ПерехЭтСек1> ПерехЭтСек1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ПерехЭтСек2> ПерехЭтСек2 { get; set; }

        //чтобы в выпадающем меню отражались названия площадок, нужно переопределить метод ToString()
        public override string ToString()
        {
            return Наименование; // возвращает название площадок
        }
    }
}
