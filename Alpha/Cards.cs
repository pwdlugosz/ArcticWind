using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticWind.Alpha
{


    public enum Suit : byte
    {
        Spade = 3,
        Heart = 2,
        Club = 1,
        Diamond = 0
    }

    public enum Value : byte
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }

    public struct Card
    {

        private Suit _Suit;
        private Value _Value;
        public const char SPADE = '♠';
        public const char HEART = '♥';
        public const char CLUB = '♣'; 
        public const char DIAMOND = '♦';

        public Card(Suit Suit, Value Value)
        {
            this._Suit = Suit;
            this._Value = Value;
        }

        public Card(byte Serial)
        {
            byte s = (byte)(Serial / 13);
            byte v = (byte)(Serial - s * 13);
            this._Suit = (Suit)s;
            this._Value = (Value)v;
        }

        public Suit Suit
        { 
            get { return this._Suit;}
        }

        public Value Value 
        { 
            get { return this._Value; } 
        }

        public override string ToString()
        {
            switch (this._Suit)
            {
                case Suit.Spade: return SPADE + " " + this._Value.ToString() + " " + SPADE;
                case Suit.Heart: return HEART + " " + this._Value.ToString() + " " + HEART;
                case Suit.Club: return CLUB + " " + this._Value.ToString() + " " + CLUB;
                case Suit.Diamond: return DIAMOND + " " + this._Value.ToString() + " " + DIAMOND;
            }
            throw new Exception();
            //return this._Value.ToString() + "Of" + this._Suit.ToString() + "s";
        }

        public byte Serial
        {
            get { return (byte)((byte)this._Suit * 13 + (byte)this._Value); }
        }

        // Statics //
        public static readonly Card AceOfSpades = new Card(Suit.Spade, Value.Ace);
        public static readonly Card TwoOfSpades = new Card(Suit.Spade, Value.Two);
        public static readonly Card ThreeOfSpades = new Card(Suit.Spade, Value.Three);
        public static readonly Card FourOfSpades = new Card(Suit.Spade, Value.Four);
        public static readonly Card FiveOfSpades = new Card(Suit.Spade, Value.Five);
        public static readonly Card SixOfSpades = new Card(Suit.Spade, Value.Six);
        public static readonly Card SevenOfSpades = new Card(Suit.Spade, Value.Seven);
        public static readonly Card EightOfSpades = new Card(Suit.Spade, Value.Eight);
        public static readonly Card NineOfSpades = new Card(Suit.Spade, Value.Nine);
        public static readonly Card TenOfSpades = new Card(Suit.Spade, Value.Ten);
        public static readonly Card JackOfSpades = new Card(Suit.Spade, Value.Jack);
        public static readonly Card QueenOfSpades = new Card(Suit.Spade, Value.Queen);
        public static readonly Card KingOfSpades = new Card(Suit.Spade, Value.King);

        public static readonly Card AceOfHearts = new Card(Suit.Heart, Value.Ace);
        public static readonly Card TwoOfHearts = new Card(Suit.Heart, Value.Two);
        public static readonly Card ThreeOfHearts = new Card(Suit.Heart, Value.Three);
        public static readonly Card FourOfHearts = new Card(Suit.Heart, Value.Four);
        public static readonly Card FiveOfHearts = new Card(Suit.Heart, Value.Five);
        public static readonly Card SixOfHearts = new Card(Suit.Heart, Value.Six);
        public static readonly Card SevenOfHearts = new Card(Suit.Heart, Value.Seven);
        public static readonly Card EightOfHearts = new Card(Suit.Heart, Value.Eight);
        public static readonly Card NineOfHearts = new Card(Suit.Heart, Value.Nine);
        public static readonly Card TenOfHearts = new Card(Suit.Heart, Value.Ten);
        public static readonly Card JackOfHearts = new Card(Suit.Heart, Value.Jack);
        public static readonly Card QueenOfHearts = new Card(Suit.Heart, Value.Queen);
        public static readonly Card KingOfHearts = new Card(Suit.Heart, Value.King);

        public static readonly Card AceOfClubs = new Card(Suit.Club, Value.Ace);
        public static readonly Card TwoOfClubs = new Card(Suit.Club, Value.Two);
        public static readonly Card ThreeOfClubs = new Card(Suit.Club, Value.Three);
        public static readonly Card FourOfClubs = new Card(Suit.Club, Value.Four);
        public static readonly Card FiveOfClubs = new Card(Suit.Club, Value.Five);
        public static readonly Card SixOfClubs = new Card(Suit.Club, Value.Six);
        public static readonly Card SevenOfClubs = new Card(Suit.Club, Value.Seven);
        public static readonly Card EightOfClubs = new Card(Suit.Club, Value.Eight);
        public static readonly Card NineOfClubs = new Card(Suit.Club, Value.Nine);
        public static readonly Card TenOfClubs = new Card(Suit.Club, Value.Ten);
        public static readonly Card JackOfClubs = new Card(Suit.Club, Value.Jack);
        public static readonly Card QueenOfClubs = new Card(Suit.Club, Value.Queen);
        public static readonly Card KingOfClubs = new Card(Suit.Club, Value.King);

        public static readonly Card AceOfDiamonds = new Card(Suit.Diamond, Value.Ace);
        public static readonly Card TwoOfDiamonds = new Card(Suit.Diamond, Value.Two);
        public static readonly Card ThreeOfDiamonds = new Card(Suit.Diamond, Value.Three);
        public static readonly Card FourOfDiamonds = new Card(Suit.Diamond, Value.Four);
        public static readonly Card FiveOfDiamonds = new Card(Suit.Diamond, Value.Five);
        public static readonly Card SixOfDiamonds = new Card(Suit.Diamond, Value.Six);
        public static readonly Card SevenOfDiamonds = new Card(Suit.Diamond, Value.Seven);
        public static readonly Card EightOfDiamonds = new Card(Suit.Diamond, Value.Eight);
        public static readonly Card NineOfDiamonds = new Card(Suit.Diamond, Value.Nine);
        public static readonly Card TenOfDiamonds = new Card(Suit.Diamond, Value.Ten);
        public static readonly Card JackOfDiamonds = new Card(Suit.Diamond, Value.Jack);
        public static readonly Card QueenOfDiamonds = new Card(Suit.Diamond, Value.Queen);
        public static readonly Card KingOfDiamonds = new Card(Suit.Diamond, Value.King);

        public static readonly Card[] Cards = new Card[]
        {

            AceOfSpades,
            TwoOfSpades,
            ThreeOfSpades,
            FourOfSpades,
            FiveOfSpades,
            SixOfSpades,
            SevenOfSpades,
            EightOfSpades,
            NineOfSpades,
            TenOfSpades,
            JackOfSpades,
            QueenOfSpades,
            KingOfSpades,
            
            AceOfHearts,
            TwoOfHearts,
            ThreeOfHearts,
            FourOfHearts,
            FiveOfHearts,
            SixOfHearts,
            SevenOfHearts,
            EightOfHearts,
            NineOfHearts,
            TenOfHearts,
            JackOfHearts,
            QueenOfHearts,
            KingOfHearts,
            
            AceOfClubs,
            TwoOfClubs,
            ThreeOfClubs,
            FourOfClubs,
            FiveOfClubs,
            SixOfClubs,
            SevenOfClubs,
            EightOfClubs,
            NineOfClubs,
            TenOfClubs,
            JackOfClubs,
            QueenOfClubs,
            KingOfClubs,
            
            AceOfDiamonds,
            TwoOfDiamonds,
            ThreeOfDiamonds,
            FourOfDiamonds,
            FiveOfDiamonds,
            SixOfDiamonds,
            SevenOfDiamonds,
            EightOfDiamonds,
            NineOfDiamonds,
            TenOfDiamonds,
            JackOfDiamonds,
            QueenOfDiamonds,
            KingOfDiamonds

        };


    }

    public class Deck
    {

        private LinkedList<Card> _Cards;
        private LinkedListNode<Card> _First;
        private LinkedListNode<Card> _Last;

        private Deck(Card[] Values)
        {
            this._Cards = new LinkedList<Card>(Values);
            this._First = this._Cards.First;
            this._Last = this._Cards.Last;
        }

        public Deck()
            : this(Card.Cards)
        {
        }

        public bool CanDeal
        {
            get { return this._First != null; }
        }

        public Card Deal()
        {
            Card c = this._First.Value;
            this._First = this._First.Next;
            return c;
        }

        public Card Peek()
        {
            return this._First.Value;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Card c in this._Cards)
            {
                sb.AppendLine(c.ToString());
            }
            return sb.ToString();
        }

        public static Card[] Shuffle(Card[] Cards, int Rounds, int Seed)
        {

            Card[] c = new Card[Cards.Length];
            Array.Copy(Cards, 0, c, 0, Cards.Length);
            Random r = new Random(Seed);

            for (int rounds = 0; rounds < Rounds; rounds++)
            {
                for (int i = 0; i < Cards.Length; i++)
                {
                    int idx = r.Next(0, Cards.Length);
                    Card x = c[i];
                    c[i] = c[idx];
                    c[idx] = x;
                }
            }

            return c;

        }

        public static Deck ShuffledDeck(int Rounds, int Seed)
        {
            return new Deck(Shuffle(Card.Cards, Rounds, Seed));
        }

        public static Deck ShuffledDeck(int Rounds)
        {
            return ShuffledDeck(Rounds, (int)Math.Abs(Guid.NewGuid().GetHashCode() ^ DateTime.Now.Ticks));
        }

        public static Deck ShuffledDeck()
        {
            return ShuffledDeck(1000);
        }


    }

    //public class Hand
    //{

    //    public Hand()
    //    {
    //    }

    //    public abstract void Encard(Card C);

    //    public abstract void Discard();

    //    public abstract IEnumerable<Card> Cards { get; }
        

    //}

    //public class TexasHoldem
    //{

    //    public class TexasHoldemHand
    //    {
    //    }

    //}

}

