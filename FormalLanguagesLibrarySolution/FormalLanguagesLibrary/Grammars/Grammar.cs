﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormalLanguagesLibrary.Grammars
{
    internal abstract class Grammar<T>
    {


        protected HashSet<Symbol<T>> _terminals = new HashSet<Symbol<T>>();
        protected HashSet<Symbol<T>> _nonTerminals = new HashSet<Symbol<T>>();
        protected Symbol<T>? _startSymbol = null;
        protected HashSet<ProductionRule<T>> _productionRules = new HashSet<ProductionRule<T>>();

        public IReadOnlyCollection<Symbol<T>> Terminals => _terminals;
        public IReadOnlyCollection<Symbol<T>> NonTerminals => _nonTerminals;
        public Symbol<T>? StartSymbol => _startSymbol;
        public IReadOnlyCollection<ProductionRule<T>> ProductionRules => _productionRules;

        public Grammar(Grammar<T> grammar)
        {
            if (grammar == null) throw new ArgumentNullException(nameof(grammar));
            foreach (var nonTerminal in grammar.NonTerminals) 
            {
                _nonTerminals.Add(nonTerminal);
            }
            foreach (var terminal in grammar.Terminals)
            {
                _terminals.Add(terminal);
            }

            _startSymbol = grammar.StartSymbol;

            foreach(var rule in grammar.ProductionRules)
            {
                _productionRules.Add(rule);
            }

            _checkInvariants();
        }

        public Grammar(IEnumerable<Symbol<T>> nonTerminals, IEnumerable<Symbol<T>> terminals, Symbol<T>? startSymbol, IEnumerable<ProductionRule<T>> productionRules)
        {
            foreach (var nonTerminal in nonTerminals)
            {
                _nonTerminals.Add(nonTerminal);
            }
            foreach (var terminal in terminals)
            {
                _terminals.Add(terminal);
            }
            _startSymbol = startSymbol;

            foreach(var rule in productionRules)
            {
                _productionRules.Add(rule);
            }

            _checkInvariants();

        }


        private void _checkInvariants()
        {
            _checkNonTerminals();
            _checkStartSymbol();
            _checkTerminals();
            _checkTerminalsAndNonTerminals();

            _checkProductionRules();
        }

        //Check the format for corresponding type of the grammar
        protected abstract void _checkFormatOfProductionRule(ProductionRule<T> rule);





        private void _checkNonTerminals()
        {
            foreach(var nonTerminal in _nonTerminals)
            {
                _checkNonTerminal(nonTerminal);
            }
        }

        private void _checkNonTerminal(Symbol<T> nonTerminal)
        {
            if(nonTerminal.Type != SymbolType.NonTerminal)
            {
                throw new GrammarException($"Invalid symbol type '{nonTerminal.Type}' for {nonTerminal.Value} symbol. Expected 'NonTerminal'.");
            }
        }


        private void _checkTerminals()
        {
            foreach (var terminal in _terminals)
            {
                _checkTerminal(terminal);
            }
        }

        private void _checkTerminal(Symbol<T> terminal)
        {
            if (terminal.Type != SymbolType.Terminal)
            {
                throw new GrammarException($"Invalid symbol type '{terminal.Type}' for {terminal.Value} symbol. Expected 'Terminal'.");
            }
        }

        private void _checkStartSymbol()
        {

            if (_startSymbol is null)
            {
                return;
            }
            if (_startSymbol?.Type != SymbolType.NonTerminal)
            {
                throw new GrammarException($"Invalid symbol type '{_startSymbol?.Type}' for the start symbol. Expected 'NonTerminal'.");
            }
            if(!_nonTerminals.Contains((Symbol<T>)_startSymbol))
            {
                throw new GrammarException($"Start symbol must be included in non-terminals of the grammar.");
            }
        }

        // Check if the intersection is empty
        private void _checkTerminalsAndNonTerminals()
        {
            var intersection = _nonTerminals.Intersect( _terminals );
            if (intersection.Any())
            {
                throw new GrammarException($"Terminals and NonTerminals should not intersect. Found common symbols {intersection}.");
            }
        }

        private void _checkProductionRules()
        {
            foreach (var rule in ProductionRules)
            {
                _checkProductionRule(rule);
                _checkFormatOfProductionRule(rule);
            }
        }

        //Check if every symbol is defined in NonTerminals or Terminals 
        private void _checkProductionRule(ProductionRule<T> rule)
        {
            foreach(var symbol in rule.LeftHandSide)
            {
                if(!_nonTerminals.Contains(symbol) && !_terminals.Contains(symbol))
                {
                    throw new GrammarException($"Symbol {symbol} is not defined in terminals and either in non-terminals.");
                }
            }

            foreach (var symbol in rule.RightHandSide)
            {
                //If the right-hand side is epsilon, continue
                if(rule.IsEpsilonRule())
                {
                    continue;
                }

                if (!_nonTerminals.Contains(symbol) && !_terminals.Contains(symbol))
                {
                    throw new GrammarException($"Symbol {symbol} is not defined in terminals and either in non-terminals.");
                }
            }
        }
    }


    public class GrammarException : Exception
    {
        public GrammarException(string message) : base(message)
        {

        }
    }



}