using System;
using System.Text.RegularExpressions;

/// <summary>
/// Manages parsing a dice roll notation into an actual dice roll
/// </summary>
public class Dice
{
    /// <summary>
    /// How many rolls
    /// </summary>
    int m_rolls;
    public int Rolls { get { return m_rolls; } }

    /// <summary>
    /// Total sides of the dice
    /// </summary>
    int m_sides;
    public int Sides { get { return m_sides; } }

    /// <summary>
    /// Additional value to add to the total roll to modify its value
    /// </summary>
    int m_modifier;
    public int Modifiers { get { return m_modifier; } }

    /// <summary>
    /// Regex captures the total <rolls> D <sides> + <modifiers> in the dice notation
    /// i.e. 
    ///     -> 1D4   : 1 roll of a 4 sided dice
    ///     -> 4D2+5 : 4 rolls of a 2 sides dice and add 5 to the results 
    /// </summary>
    Regex m_regex;
    string m_pattern = "(?<rolls>[1-9]\\d*)D(?<sides>[1-9]\\d*)?([\\+-](?<modifier>[1-9]\\d*))?";


    /// <summary>
    /// Keeps track of the random number generation's seed and position
    /// </summary>
    Random m_random;

    /// <summary>
    /// Creates the dice with the given notation
    /// If notation is not given then the dice defaults to:
    ///     1 roll
    ///     1 side
    ///     0 moidifier
    /// </summary>
    /// <param name="notation"></param>
    public Dice(string notation = "")
    {
        m_rolls = 1;
        m_sides = 1;
        m_modifier = 0;

        m_regex = new Regex(m_pattern, RegexOptions.IgnoreCase);
        m_random = new Random(Guid.NewGuid().GetHashCode());

        if (!string.IsNullOrEmpty(notation)) {
            Parse(notation);
        }
    }

    /// <summary>
    /// Parses the given dice notation into a its rolls, sides, and modifier
    /// </summary>
    /// <param name="notation"></param>
    public void Parse(string notation)
    {
        Match match = m_regex.Match(notation);

        if (match.Success) {
            GroupCollection groups = match.Groups;

            int.TryParse(groups["rolls"].ToString(), out m_rolls);
            int.TryParse(groups["sides"].ToString(), out m_sides);
            int.TryParse(groups["modifier"].ToString(), out m_modifier);
        }
    }

    /// <summary>
    /// Rolls the given dice and returns the results of the roll
    /// Rolls are affected by the dice's total rolls/sides and modifier
    /// </summary>
    /// <param name="dice"></param>
    /// <returns></returns>
    public int Roll()
    {
        int total = 0;

        for (int i = 0; i < Rolls; i++) {
            // +1 as we .Next will never yield the higest number
            // but we want the highest number
            total += m_random.Next(1, Sides + 1);
        }

        return total + Modifiers;
    }
}
