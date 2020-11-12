﻿using System;

namespace PubComp.NoSql.Core
{
    public class IndexDefinition
    {
        public IndexDefinition(Type entityType, KeyProperty[] fields, bool asUnique, bool asSparse)
        {
            this.EntityType = entityType;
            this.Fields = fields;
            this.AsUnique = asUnique;
            this.AsSparse = asSparse;
        }

        public IndexDefinition(Type entityType, KeyProperty[] fields, bool asUnique)
        {
            this.EntityType = entityType;
            this.Fields = fields;
            this.AsUnique = asUnique;
            this.AsSparse = false;
        }

        public Type EntityType { private set; get; }
        public KeyProperty[] Fields { private set; get; }
        public bool AsUnique { private set; get; }
        public bool AsSparse { private set; get; }
    }

    public class KeyProperty
    {
        public KeyProperty(string name, Direction direction)
        {
            this.Name = name;
            this.Direction = direction;
        }

        public string Name { private set; get; }
        public Direction Direction { private set; get; }
    }

    public enum Direction
    {
        Ascending,
        Descending,
    }
}
