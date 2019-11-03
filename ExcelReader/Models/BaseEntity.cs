using System;
using ExcelReader.Enums;

namespace ExcelReader.Models
{
    public abstract class BaseEntity
    {
        protected BaseEntity(Guid id)
        {
            Id = id;
            Availability = MasterItemAvailabilities.Available;
        }

        #region Properties

        /// <summary>
        /// Id of entity.
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public Guid Id { get; private set; }

        /// <summary>
        /// Whether item is available or not.
        /// </summary>
        public MasterItemAvailabilities Availability { get; set; }

        /// <summary>
        /// Time when record was created.
        /// </summary>
        public double CreatedTime { get; set; }

        /// <summary>
        /// Time when record was lastly modified.
        /// </summary>
        public double? LastModifiedTime { get; set; }

        #endregion
    }
}