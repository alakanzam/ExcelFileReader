using System;

namespace ExcelReader.Models
{
    public class State : BaseEntity
    {
        #region Properties

        public string Name { get; set; }

        public double DeliveryFee { get; set; }

        public Guid CreatorId { get; set; }

        public Guid? ModifierId { get; set; }

        #endregion

        #region Constructor

        public State(Guid id) : base(id)
        {
        }

        #endregion
    }
}