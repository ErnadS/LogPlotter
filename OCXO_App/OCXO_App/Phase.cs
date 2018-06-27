using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCXO_App
{
    public class Phase
    {
        // !!!HELP: ovo je vrijednost faze u sekundama ili u sec/10? Ona se prikazuje na grafu.
        public static double calculatePhaseFromInputString(string input)   // staticka funkcija jer ne postavlja nikakve member varijable. Samo primi nesto i vrati rezultat. Obzirom da je staticka, ne treba nam objekat
        {
            string[] subInputs = input.Split(',');
            List<Int32> inputValues = new List<Int32>();
            double phase = 0;
            foreach (string s in subInputs)
            {
                inputValues.Add(Int32.Parse(s));
            }
            if (inputValues.Count == 5)
            {
                // inputValues[2] ima 32-bitni brojac u FPGA koji broji impulse od 400MHz (ili 200). =>
                // inputValues[2] / 400000000 je vrijeme u sekundama izmedju eksternog i generisanog PPS-a. To je grubo mjerenje.
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Za fino mjerenje trebamo jos dodati vrijeme od prvog PPS-a koji je dosao, do prvog 400MHz impulsa:
                // inputValues[3] * (20 * Math.Pow(10, (-11)))
                // inputValues[3] je broj celija a 20*10^-11 je vrijeme prebacivanja iz celije u celiju
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Na to jos trebamo dodati vrijeme od zadnjeg izmjerenog clock pa do drugog PPS-a koji je dosao. To vrijeme nemamo
                // ali imamo vrijeme od drugog PPS-a pa do sljedeceg clock 400MHz. Taj impuls nije unutar mjerenja i nama treba vrijeme do prethodnog 400MHz.
                // Zato od perioda 400MHz oduzimamo izmjereno vrijeme:
                // 2.5 * Math.Pow(10, -9) - inputValues[4] * (20 * Math.Pow(10, -11))
                // Period je 2.5 * Math.Pow(10, -9)
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (inputValues[0] == 0)   // inputValues[0] = 0, znaci da je prvo dosao externi PPS
                {
                    if (inputValues[1] == 0)  // inputValues[1] == 0 znaci faza manja od 100us (ili 50?). Onda ima smisla da koristimo i fino mjerenje u racunici
                    {
                        if (inputValues[2] == 0) // nije izmjeren ni jedan impuls => Oba PPS-a u finom mjerenju imaju vrijeme do istog 400MHz impulsa (ne trebamo u racunici period od 400MHz (2.5 * Math.Pow(10, -9))
                        {
                            phase = (inputValues[3] * (20 * Math.Pow(10, (-11))) - inputValues[4] * (20 * Math.Pow(10, -11))) * (-1);
                        }
                        else
                        {
                            phase = ((Convert.ToDouble(inputValues[2]) / 400000000) + inputValues[3] * (20 * Math.Pow(10, (-11))) + 2.5 * Math.Pow(10, -9)
                                                                                - inputValues[4] * (20 * Math.Pow(10, -11))) * (-1);
                        }                   
                    }
                    else // faza je veca od 100us, nema potrebe koristiti fino mjerenje i koristen je clock od 200MHz
                    {
                        phase = (Convert.ToDouble(inputValues[2])) / 200000000 * (-1); // 200MHz sampling
                    }
                }
                else
                {
                    if (inputValues[1] == 0)
                    {
                        if (inputValues[2] == 0) // nije izmjeren ni jedan impuls => Oba PPS-a u finom mjerenju imaju vrijeme do istog 400MHz impulsa (ne trebamo u racunici period od 400MHz (2.5 * Math.Pow(10, -9))
                        {
                            phase = (inputValues[3] * (20 * Math.Pow(10, (-11))) - inputValues[4] * (20 * Math.Pow(10, -11)));
                        }
                        else
                        {
                            phase = ((Convert.ToDouble(inputValues[2]) / 400000000) + inputValues[3] * (20 * Math.Pow(10, (-11))) + 2.5 * Math.Pow(10, -9)
                                                                                    - inputValues[4] * (20 * Math.Pow(10, -11)));
                        }
                    }
                    else
                    {
                        phase = (Convert.ToDouble(inputValues[2])) / 200000000;
                    }
                }
            }
            return phase;
        }


        public static double calculatePhaseFromInputString_B(string input)
        {
            string[] subInputs = input.Split(',');
            List<Int32> inputValues = new List<Int32>();
            double phase = 0;
            /****************************************
             * Racunanje faze iz primljene poruke ali grubo i rezultat je cijeli broj (nije u sekundama nego sec/400000000) (ili sec/200000000) 
             ****************************************/
            foreach (string s in subInputs)
            {
                inputValues.Add(Int32.Parse(s));
            }
            phase = inputValues[2];

            if (inputValues[0] == 0)
            {
                phase *= (-1);
            }

            return phase;
        }
    }
}
