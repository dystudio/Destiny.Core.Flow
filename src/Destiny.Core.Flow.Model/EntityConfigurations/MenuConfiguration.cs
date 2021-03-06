﻿using Destiny.Core.Flow.Model.Entities.Menu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Destiny.Core.Flow.Model.EntityConfigurations
{
    public class MenuConfiguration : EntityMappingConfiguration<MenuEntity, Guid>
    {
        public override void Map(EntityTypeBuilder<MenuEntity> b)
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(50).IsRequired();
            b.Property(x => x.ParentId).HasDefaultValue(Guid.Empty);
            b.Property(x => x.RouterPath).HasMaxLength(200).IsRequired();
            b.Property(x => x.Sort).HasDefaultValue(0);
            b.Property(x => x.Iocn).HasMaxLength(50);
            b.Property(x => x.IsDeleted).HasDefaultValue(0);
            b.ToTable("Menu");
        }
    }
}
