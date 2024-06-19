﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace FormalLanguagesLibrary.Grammars
{
    internal struct Symbol<T>(T? value, SymbolType type) : IEquatable<Symbol<T>>
    {
        public T? Value { get; } = value;
        public SymbolType Type { get; } = type;

        // Constant to create an Epsilon symbol
        public static readonly Symbol<T> Epsilon = new Symbol<T>(default, SymbolType.Epsilon);

        public override bool Equals(object? obj) => obj is Symbol<T> other && this.Equals(other);

        //Compares only values!
        public bool Equals(Symbol<T> other)
        {
            // For epsilons it doesn't depend on the value of the symbol
            if (Type == SymbolType.Epsilon && other.Type == SymbolType.Epsilon)
                return true;

            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            // HashCode of two Epsilon symbols must be always Equal
            if (Type == SymbolType.Epsilon) return SymbolType.Epsilon.GetHashCode();

            return Value?.GetHashCode() ?? 0;
        }

        public static bool operator ==(Symbol<T> symbol1, Symbol<T> symbol2) => symbol1.Equals(symbol2);
        public static bool operator !=(Symbol<T> symbol1, Symbol<T> symbol2) => !(symbol1 == symbol2);

        public override string ToString()
        {
            return Value?.ToString() ?? "";
        }

    }

    public enum SymbolType { NonTerminal, Terminal, Epsilon}
}
