using System;

namespace ExcelReader.Models
{
    public class City : BaseEntity
    {
        #region Properties

        public Guid StateId { get; set; }

        public string Name { get; set; }

        #endregion

        #region Constructors

        public City(Guid id) : base(id)
        {
        }

        #endregion
    }
}