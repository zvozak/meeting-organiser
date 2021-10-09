using CommonData.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonData.DTOs
{
    public class SpecialNeedDTO
    {
        public Int32 Id { get; set; }

        public Boolean IsSelected { get; set; }
        public override Boolean Equals(Object obj)
        {
            return (obj is SpecialNeedDTO dto) &&
                   Id == dto.Id &&
                   IsSelected == dto.IsSelected;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static SpecialNeedDTO[] Convert(SpecialNeed features)
        {
            List<SpecialNeedDTO> result = new List<SpecialNeedDTO>();

            Int32 featureId = 0;
            foreach (SpecialNeed feature in Enum.GetValues(typeof(SpecialNeed)))
            {
                if (feature > 0)
                {
                    result.Add(new SpecialNeedDTO
                    {
                        Id = featureId++,
                        IsSelected = features.HasFlag(feature)
                    });
                }
            }

            return result.ToArray();
        }

        public static SpecialNeed Convert(SpecialNeedDTO[] features)
        {
            if (features == null || features.Length == 0)
                return SpecialNeed.None;

            SpecialNeed result = SpecialNeed.None;
            foreach (var feature in features)
            {
                if (feature.IsSelected)
                {
                    result += (1 << feature.Id-1);
                }
            }

            return result;
        }
    }
}
