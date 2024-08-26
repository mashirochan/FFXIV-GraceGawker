using System.Collections.Generic;

namespace GraceGawker
{
    public class DataModels
    {
        public static readonly List<uint> NonCombatJobIds = [
            8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18
            ];

        public enum PlayerState
        {
            Crafting,
            Gathering,
            Inactive
        }

        public enum ManualType
        {
            Crafting,
            Gathering
        }

        public class Manual
        {
            public int Id { get; set; }
            public ManualType Type { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int Duration { get; set; }
            public int PenaltyLevel { get; set; }
            public int MaxExp { get; set; }
            public float ExpBoost { get; set; }
        }

        public static readonly List<Manual> Manuals =
        [
            /*
             *          CRAFTING MANUALS
             */

            new Manual
            {
                Id = 4632,
                Type = ManualType.Crafting,
                Name = "Company-issue Engineering Manual",
                Description = "This comprehensive manual on crafting contains knowledge amassed over centuries by well-traveled Disciples of the Hand. It is known to inspire all who read it, granting a temporary 150% boost to experience points earned from synthesis (up to a maximum of 250,000 points). Effect is halved at level 40 and above.",
                Duration = 18 * 60 * 60,
                PenaltyLevel = 40,
                MaxExp = 250000,
                ExpBoost = 1.5f
            },
            new Manual
            {
                Id = 4634,
                Type = ManualType.Crafting,
                Name = "Company-issue Engineering Manual II",
                Description = "The second in a series of comprehensive manuals on crafting containing knowledge amassed over centuries by well-traveled Disciples of the Hand. It is known to inspire all who read it, granting a temporary 150% boost to experience points earned from synthesis (up to a maximum of 500,000 points). Effect is halved at level 50 and above.",
                Duration = 18 * 60 * 60,
                PenaltyLevel = 50,
                MaxExp = 500000,
                ExpBoost = 1.5f
            },
            new Manual
            {
                Id = 12667,
                Type = ManualType.Crafting,
                Name = "Commercial Engineering Manual",
                Description = "This comprehensive manual, written by a disgruntled former employee of Rowena's House of Splendors, provides a detailed explanation of inhuman techniques forced upon consortium-hired Disciples of the Hand to increase their output. It is known to inspire (and frighten) all who read it, granting a temporary 150% boost to experience points earned from synthesis (up to a maximum of 1,750,000 points). Effect is halved at level 70 and above.",
                Duration = 18 * 60 * 60,
                PenaltyLevel = 70,
                MaxExp = 1750000,
                ExpBoost = 1.5f
            },
            new Manual
            {
                Id = 26554,
                Type = ManualType.Crafting,
                Name = "Revised Engineering Manual",
                Description = "This comprehensive manual on crafting─now entirely rendered in modern languages and free of lacunae─contains knowledge amassed over centuries by well-traveled Disciples of the Hand. It is known to inspire all who read it, granting a temporary 150% boost to experience points earned from synthesis(up to a maximum of 2,000,000 points). Effect is halved at level 80 and above.",
                Duration = 18 * 60 * 60,
                PenaltyLevel = 80,
                MaxExp = 2000000,
                ExpBoost = 1.5f
            },

            /*
             *          GATHERING MANUALS
             */

            new Manual
            {
                Id = 4633,
                Type = ManualType.Gathering,
                Name = "Company-issue Survival Manual",
                Description = "This comprehensive manual on survival techniques contains knowledge amassed over centuries by trackers, hunters, and woodsmen. It is known to inspire all who read it, granting a temporary 150% boost to experience points earned from gathering (up to a maximum of 250,000 points). Effect is halved at level 40 and above.",
                Duration = 18 * 60 * 60,
                PenaltyLevel = 40,
                MaxExp = 250000,
                ExpBoost = 1.5f
            },
            new Manual
            {
                Id = 4635,
                Type = ManualType.Gathering,
                Name = "Company-issue Survival Manual II",
                Description = "The second in a series of comprehensive manuals on survival techniques containing knowledge amassed over centuries by trackers, hunters, and woodsmen. It is known to inspire all who read it, granting a temporary 150% boost to experience points earned from gathering (up to a maximum of 500,000 points). Effect is halved at level 50 and above.",
                Duration = 18 * 60 * 60,
                PenaltyLevel = 50,
                MaxExp = 500000,
                ExpBoost = 1.5f
            },
            new Manual
            {
                Id = 12668,
                Type = ManualType.Gathering,
                Name = "Commercial Survival Manual",
                Description = "This comprehensive manual, written by a disgruntled former employee of Rowena's House of Splendors, provides a detailed explanation of inhuman techniques forced upon consortium-hired Disciples of the Land to increase their output. It is known to inspire (and frighten) all who read it, granting a temporary 150% boost to experience points earned from gathering activities (up to a maximum of 1,750,000 points). Effect is halved at level 70 and above.",
                Duration = 18 * 60 * 60,
                PenaltyLevel = 70,
                MaxExp = 1750000,
                ExpBoost = 1.5f
            },
            new Manual
            {
                Id = 26553,
                Type = ManualType.Gathering,
                Name = "Revised Survival Manual",
                Description = "This comprehensive manual on survival techniques─now entirely rendered in modern languages and free of lacunae─contains knowledge amassed over centuries by trackers, hunters, and woodsmen. It is known to inspire all who read it, granting a temporary 150% boost to experience points earned from gathering activities (up to a maximum of 2,000,000 points). Effect is halved at level 80 and above.",
                Duration = 18 * 60 * 60,
                PenaltyLevel = 80,
                MaxExp = 2000000,
                ExpBoost = 1.5f
            }
        ];
    }
}
