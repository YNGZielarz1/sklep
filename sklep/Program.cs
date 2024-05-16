using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

class Produkt
{
    public string Nazwa { get; set; }
    public string Producent { get; set; }
    public decimal Cena { get; set; }
    public string Kategoria { get; set; }
    public int Ilość { get; set; }
    public DateTime DataDostawy { get; set; }
}

class Zamówienie
{
    public Dictionary<Produkt, int> ListaProduktów { get; set; }
    public string Imię { get; set; }
    public string Nazwisko { get; set; }
    public string Adres { get; set; }
    public string SposóbDostawy { get; set; } 
    public string SposóbPłatności { get; set; } 
    public decimal KwotaCałkowita { get; set; }

    public void ObliczKwotęCałkowitą()
    {
        KwotaCałkowita = 0;
        foreach (var kvp in ListaProduktów)
        {
            KwotaCałkowita += kvp.Key.Cena * kvp.Value;
        }
        if (SposóbDostawy == "kurier")
            KwotaCałkowita += 20;
        if (SposóbPłatności == "karta")
            KwotaCałkowita += 2;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Witaj w sklepie!");
        Console.WriteLine("Dostępne produkty:");
        List<Produkt> produkty = WczytajProduktyZPliku(@"C:\Users\tymek\source\repos\sklep\sklep\produkty.json");
        if (produkty == null)
        {
            Console.WriteLine("Nie udało się wczytać produktów. Sklep jest chwilowo niedostępny.");
            return;
        }
        foreach (var produkt in produkty)
        {
            Console.WriteLine($"{produkt.Nazwa} - {produkt.Cena} zł");
        }

        Zamówienie zamówienie = UtwórzNoweZamówienie(produkty);
        if (zamówienie == null)
        {
            Console.WriteLine("Nie udało się złożyć zamówienia. Spróbuj ponownie później.");
            return;
        }

        WyświetlSzczegółyZamówienia(zamówienie);

        if (ZapiszZamówienieDoPliku(zamówienie, "zamowienia.json"))
        {
            Console.WriteLine("Twoje zamówienie zostało pomyślnie złożone i zapisane.");
        }
        else
        {
            Console.WriteLine("Nie udało się zapisać zamówienia. Skontaktuj się z obsługą klienta.");
        }
    }

    static List<Produkt> WczytajProduktyZPliku(string nazwaPliku)
    {
        try
        {
            string jsonData = File.ReadAllText(nazwaPliku);
            return JsonConvert.DeserializeObject<List<Produkt>>(jsonData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd podczas wczytywania produktów: {ex.Message}");
            return null;
        }
    }

    static Zamówienie UtwórzNoweZamówienie(List<Produkt> produkty)
    {
        Zamówienie zamówienie = new Zamówienie();
        zamówienie.ListaProduktów = new Dictionary<Produkt, int>();

        while (true)
        {
            Console.WriteLine("Wybierz produkt, który chcesz dodać do zamówienia (wpisz numer z listy lub 0, aby zakończyć):");
            for (int i = 0; i < produkty.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {produkty[i].Nazwa} - {produkty[i].Cena} zł");
            }
            Console.Write("Twój wybór: ");
            string wybór = Console.ReadLine();
            if (wybór == "0")
            {
                break;
            }
            else if (int.TryParse(wybór, out int numerProduktu) && numerProduktu > 0 && numerProduktu <= produkty.Count)
            {
                Console.Write("Podaj ilość: ");
                if (int.TryParse(Console.ReadLine(), out int ilość) && ilość > 0)
                {
                    Produkt wybranyProdukt = produkty[numerProduktu - 1];
                    zamówienie.ListaProduktów.Add(wybranyProdukt, ilość);
                }
                else
                {
                    Console.WriteLine("Nieprawidłowa ilość. Spróbuj ponownie.");
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowy wybór. Spróbuj ponownie.");
            }
        }

        Console.Write("Podaj swoje imię: ");
        zamówienie.Imię = Console.ReadLine();

        Console.Write("Podaj swoje nazwisko: ");
        zamówienie.Nazwisko = Console.ReadLine();

        Console.Write("Podaj swój adres: ");
        zamówienie.Adres = Console.ReadLine();

        Console.Write("Wybierz sposób dostawy (wpisz 'odbior' lub 'kurier'): ");
        zamówienie.SposóbDostawy = Console.ReadLine();

        Console.Write("Wybierz sposób płatności (wpisz 'gotówka' lub 'karta'): ");
        zamówienie.SposóbPłatności = Console.ReadLine();

        zamówienie.ObliczKwotęCałkowitą();

        return zamówienie;
    }

    static void WyświetlSzczegółyZamówienia(Zamówienie zamówienie)
    {
        Console.WriteLine("\nSzczegóły zamówienia:");
        Console.WriteLine("Produkty:");
        foreach (var kvp in zamówienie.ListaProduktów)
        {
            Console.WriteLine($"{kvp.Key.Nazwa} - {kvp.Value} szt. - {kvp.Key.Cena * kvp.Value} zł");
        }
        Console.WriteLine($"Imię: {zamówienie.Imię}");
        Console.WriteLine($"Nazwisko: {zamówienie.Nazwisko}");
        Console.WriteLine($"Adres: {zamówienie.Adres}");
        Console.WriteLine($"Sposób dostawy: {zamówienie.SposóbDostawy}");
        Console.WriteLine($"Sposób płatności: {zamówienie.SposóbPłatności}");
        Console.WriteLine($"Kwota całkowita: {zamówienie.KwotaCałkowita} zł");
    }

    static bool ZapiszZamówienieDoPliku(Zamówienie zamówienie, string nazwaPliku)
    {
        try
        {
            string jsonData = JsonConvert.SerializeObject(zamówienie, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(nazwaPliku, jsonData);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd podczas zapisywania zamówienia: {ex.Message}");
            return false;
        }
    }
}
