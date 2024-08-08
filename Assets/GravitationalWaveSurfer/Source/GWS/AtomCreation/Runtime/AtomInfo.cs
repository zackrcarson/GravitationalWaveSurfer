using GWS.AtomCreation.Runtime;
using GWS.UI.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public readonly struct AtomInfo 
{
    public AtomInfo(Outcome atomicSymbol, string fullName, int protons, int neutrons, int electrons, double mass)
    {
        Atom = atomicSymbol;
        FullName = fullName;
        Protons = protons;
        Neutrons = neutrons;
        Electrons = electrons;
        Mass = mass;
    }

    public Outcome Atom { get; }
    public string FullName { get;  }
    public int Protons { get; }
    public int Neutrons { get; }
    public int Electrons { get; }
    public double Mass { get; }

    public string Name() => Atom.ToString().Substring(0, 3);

    public static readonly AtomInfo Hydrogen1 = new AtomInfo(Outcome.H, "Hydrogen", 1, 0, 1, 1.00784);
    public static readonly AtomInfo Hydrogen2 = new AtomInfo(Outcome.H2, "Deuterium", 1, 1, 1, 2.01410);
    public static readonly AtomInfo Helium3 = new AtomInfo(Outcome.He3, "Helium-3", 2, 1, 2, 3.01603);
    public static readonly AtomInfo Helium4 = new AtomInfo(Outcome.He4, "Helium-4", 2, 2, 2, 4.00260);
    public static readonly AtomInfo Carbon = new AtomInfo(Outcome.C, "Carbon", 6, 6, 6, 12.0107);
    public static readonly AtomInfo Oxygen = new AtomInfo(Outcome.O, "Oxygen", 8, 8, 8, 15.9994);
    public static readonly AtomInfo Iron = new AtomInfo(Outcome.Fe, "Iron", 26, 30, 26, 55.8452);
    public static readonly AtomInfo Lithium = new AtomInfo(Outcome.Li, "Lithium", 3, 4, 3, 6.94);
    public static readonly AtomInfo Beryllium = new AtomInfo(Outcome.Be, "Beryllium", 4, 5, 4, 9.01218);
    public static readonly AtomInfo Boron = new AtomInfo(Outcome.B, "Boron", 5, 6, 5, 10.81);
    public static readonly AtomInfo Nitrogen = new AtomInfo(Outcome.N, "Nitrogen", 7, 7, 7, 14.0067);
    public static readonly AtomInfo Fluorine = new AtomInfo(Outcome.F, "Fluorine", 9, 10, 9, 18.9984);
    public static readonly AtomInfo Neon = new AtomInfo(Outcome.Ne, "Neon", 10, 10, 10, 20.1797);
    public static readonly AtomInfo Sodium = new AtomInfo(Outcome.Na, "Sodium", 11, 12, 11, 22.9897);
    public static readonly AtomInfo Magnesium = new AtomInfo(Outcome.Mg, "Magnesium", 12, 12, 12, 24.3050);
    public static readonly AtomInfo Aluminum = new AtomInfo(Outcome.Al, "Aluminum", 13, 14, 13, 26.9815);
    public static readonly AtomInfo Silicon = new AtomInfo(Outcome.Si, "Silicon", 14, 14, 14, 28.0855);
    public static readonly AtomInfo Phosphorus = new AtomInfo(Outcome.P, "Phosphorus", 15, 16, 15, 30.9738);
    public static readonly AtomInfo Sulfur = new AtomInfo(Outcome.S, "Sulfur", 16, 16, 16, 32.065);
    public static readonly AtomInfo Chlorine = new AtomInfo(Outcome.Cl, "Chlorine", 17, 18, 17, 35.453);
    public static readonly AtomInfo Argon = new AtomInfo(Outcome.Ar, "Argon", 18, 22, 18, 39.948);
    public static readonly AtomInfo Potassium = new AtomInfo(Outcome.K, "Potassium", 19, 20, 19, 39.0983);
    public static readonly AtomInfo Calcium = new AtomInfo(Outcome.Ca, "Calcium", 20, 20, 20, 40.078);
    public static readonly AtomInfo Scandium = new AtomInfo(Outcome.Sc, "Scandium", 21, 24, 21, 44.9559);
    public static readonly AtomInfo Titanium = new AtomInfo(Outcome.Ti, "Titanium", 22, 26, 22, 47.867);
    public static readonly AtomInfo Vanadium = new AtomInfo(Outcome.V, "Vanadium", 23, 28, 23, 50.9415);
    public static readonly AtomInfo Chromium = new AtomInfo(Outcome.Cr, "Chromium", 24, 28, 24, 51.9961);
    public static readonly AtomInfo Manganese = new AtomInfo(Outcome.Mn, "Manganese", 25, 30, 25, 54.9380);
    public static readonly AtomInfo Cobalt = new AtomInfo(Outcome.Co, "Cobalt", 27, 32, 27, 58.9332);
    public static readonly AtomInfo Nickel = new AtomInfo(Outcome.Ni, "Nickel", 28, 31, 28, 58.6934);
    public static readonly AtomInfo Copper = new AtomInfo(Outcome.Cu, "Copper", 29, 35, 29, 63.546);
    public static readonly AtomInfo Zinc = new AtomInfo(Outcome.Zn, "Zinc", 30, 35, 30, 65.38);
    public static readonly AtomInfo Gallium = new AtomInfo(Outcome.Ga, "Gallium", 31, 39, 31, 69.723);
    public static readonly AtomInfo Germanium = new AtomInfo(Outcome.Ge, "Germanium", 32, 41, 32, 72.64);
    public static readonly AtomInfo Arsenic = new AtomInfo(Outcome.As, "Arsenic", 33, 42, 33, 74.9216);
    public static readonly AtomInfo Selenium = new AtomInfo(Outcome.Se, "Selenium", 34, 45, 34, 78.96);
    public static readonly AtomInfo Bromine = new AtomInfo(Outcome.Br, "Bromine", 35, 45, 35, 79.904);
    public static readonly AtomInfo Krypton = new AtomInfo(Outcome.Kr, "Krypton", 36, 48, 36, 83.798);
    public static readonly AtomInfo Rubidium = new AtomInfo(Outcome.Rb, "Rubidium", 37, 48, 37, 85.4678);
    public static readonly AtomInfo Strontium = new AtomInfo(Outcome.Sr, "Strontium", 38, 50, 38, 87.62);
    public static readonly AtomInfo Yttrium = new AtomInfo(Outcome.Y, "Yttrium", 39, 50, 39, 88.9059);
    public static readonly AtomInfo Zirconium = new AtomInfo(Outcome.Zr, "Zirconium", 40, 51, 40, 91.224);

    /// <summary>
    /// Order in which elements should be unlocked by the player and is more accurate to star fusion. Some elements from actual star fusion are omitted.
    /// </summary>
    public static readonly AtomInfo[] Order =
    {
            Hydrogen1,
            Hydrogen2,
            Helium3,
            Helium4,
            Carbon,
            Oxygen,
            Iron,
    };

    public static readonly AtomInfo[] AllRandomAtoms =
    {
            Lithium,
            Beryllium,
            Boron,
            Nitrogen,
            Fluorine,
            Neon,
            Sodium,
            Magnesium,
            Aluminum,
            Silicon,
            Phosphorus,
            Sulfur,
            Chlorine,
            Argon,
            Potassium,
            Calcium,
            Scandium,
            Titanium,
            Vanadium,
            Chromium,
            Manganese,
            Cobalt,
            Nickel,
            Copper,
            Zinc,
            Gallium,
            Germanium,
            Arsenic,
            Selenium,
            Bromine,
            Krypton,
            Rubidium,
            Strontium,
            Yttrium,
            Zirconium,
    };

    public static readonly AtomInfo[] AllAtoms = Order.Concat(AllRandomAtoms).ToArray();
}
