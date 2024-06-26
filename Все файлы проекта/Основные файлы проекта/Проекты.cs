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
    
    public partial class Проекты
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Проекты()
        {
            this.Колонны = new HashSet<Колонны>();
            this.Перекрытия = new HashSet<Перекрытия>();
            this.Переходные = new HashSet<Переходные>();
            this.Стены = new HashSet<Стены>();
        }
    
        public int IDПроекта { get; set; }
        public string Обозначение { get; set; }
        public string Наименование { get; set; }
        public Nullable<decimal> НомерИзменений { get; set; }
        public Nullable<System.DateTime> ДатаВыдачи { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Колонны> Колонны { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Перекрытия> Перекрытия { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Переходные> Переходные { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Стены> Стены { get; set; }
    }
}
