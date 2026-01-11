using System;
using System.Collections.Generic;

using System.IO;
using System.Text.Json;


// Applicazione della biblioteca: gestisce menu, input/iutput, coordinamento tra oggetti
class LibraryApp
{
    private List<Book> books = new List<Book>();
    private List<User> users = new List<User>();
    private List<Loan> loans = new List<Loan>();

    private const string BooksFile = "books.json";
    private const string UsersFile = "users.json";
    private const string LoansFile = "loans.json";

    // testo indentato nei file .json
    private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    private void LoadData()
    {
        if (File.Exists(BooksFile))
        {
            string json = File.ReadAllText(BooksFile);
            books = JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }

        if (File.Exists(UsersFile))
        {
            string json = File.ReadAllText(UsersFile);
            users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        if (File.Exists(LoansFile))
        {
            string json = File.ReadAllText(LoansFile);
            loans = JsonSerializer.Deserialize<List<Loan>>(json) ?? new List<Loan>();
        }
    }

    private void SaveData()
    {
        File.WriteAllText(BooksFile, JsonSerializer.Serialize(books, jsonOptions));
        File.WriteAllText(UsersFile, JsonSerializer.Serialize(users, jsonOptions));
        File.WriteAllText(LoansFile, JsonSerializer.Serialize(loans, jsonOptions));

        // per visualizzare la directory di salvataggio dei dati .json
        Console.WriteLine($"Salvataggio in: {Directory.GetCurrentDirectory()}");

    }

    public void Run()
    {
        LoadData();

        bool running = true;
        while (running)
        {
            ShowMenu();
            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1": AddBook(); break;
                case "2": ShowBooks(); break;
                case "3": AddUser(); break;
                case "4": ShowUsers(); break;
                case "5": AddLoan(); break;
                case "6": ShowLoans(); break;
                case "0":
                    SaveData();
                    running = false;
                    break;
                default:
                    Console.WriteLine("Scelta non valida.");
                    break;
            }
        }
    }

    private void ShowMenu()
    {
        Console.WriteLine("\n=== Library Manager ===");
        Console.WriteLine("1 - Aggiungi libro");
        Console.WriteLine("2 - Mostra libri");
        Console.WriteLine("3 - Aggiungi utente");
        Console.WriteLine("4 - Mostra utenti");
        Console.WriteLine("5 - Effettua prestito");
        Console.WriteLine("6 - Mostra prestiti");
        Console.WriteLine("0 - Esci");
        Console.Write("Scelta: ");
    }

    private void AddBook()
    {
        Book book = new Book();

        Console.Write("Titolo: ");
        book.Title = Console.ReadLine() ?? "";

        Console.Write("Autore: ");
        book.Author = Console.ReadLine() ?? "";

        Console.Write("Anno di pubblicazione: ");
        book.PublicationYear = int.Parse(Console.ReadLine() ?? "0");

        books.Add(book);
        Console.WriteLine("Libro aggiunto!");
    }

    private void ShowBooks()
    {
        if (books.Count == 0)
        {
            Console.WriteLine("Nessun libro presente.");
            return;
        }

        foreach (Book b in books)
        {
            Console.WriteLine($"{b.Title} - {b.Author} ({b.PublicationYear})");
        }
    }

        private void AddUser()
    {
        User user = new User();

        Console.Write("Nome: ");
        user.Name = Console.ReadLine() ?? "";

        Console.Write("Cognome: ");
        user.Surname = Console.ReadLine() ?? "";

        users.Add(user);
        Console.WriteLine("Utente aggiunto!");
    }

    private void ShowUsers()
    {
        if (users.Count == 0)
        {
            Console.WriteLine("Nessun utente presente.");
            return;
        }

        foreach (User u in users)
        {
            Console.WriteLine($"{u.Name} {u.Surname}");
        }
    }

        private void AddLoan()
    {
        if (books.Count == 0 || users.Count == 0)
        {
            Console.WriteLine("Libri o utenti non disponibili.");
            return;
        }

        Console.WriteLine("Seleziona libro:");
        for (int i = 0; i < books.Count; i++)
            Console.WriteLine($"{i} - {books[i].Title}");

        int bookIndex = int.Parse(Console.ReadLine() ?? "0");

        Console.WriteLine("Seleziona utente:");
        for (int i = 0; i < users.Count; i++)
            Console.WriteLine($"{i} - {users[i].Name} {users[i].Surname}");

        int userIndex = int.Parse(Console.ReadLine() ?? "0");

        Loan loan = new Loan
        {
            BookId = books[bookIndex].Id,
            UserId = users[userIndex].Id,
            DueDate = DateTime.Now.AddDays(14)
        };

        loans.Add(loan);
        Console.WriteLine("Prestito registrato!");
    }

    private void ShowLoans()
    {
        if (loans.Count == 0)
        {
            Console.WriteLine("Nessun prestito presente.");
            return;
        }

        foreach (Loan l in loans)
        {
            // uso i ! perché so che non posso trovare valori null per come ho dichiarato loans
            Book b = books.Find(x => x.Id == l.BookId)!;
            User u = users.Find(x => x.Id == l.UserId)!;

            Console.WriteLine($"{b.Title} → {u.Name} {u.Surname} (scadenza {l.DueDate:d})");
        }
    }
}



