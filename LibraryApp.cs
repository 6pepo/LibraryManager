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
        //Console.WriteLine($"Salvataggio in: {Directory.GetCurrentDirectory()}");

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
                case "7": ReturnLoan(); break;
                case "8": ShowOverdueLoans(); break;

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
        Console.WriteLine("7 - Restituisci libro");
        Console.WriteLine("8 - Mostra prestiti scaduti");
        Console.WriteLine("0 - Esci");
        Console.Write("Scelta: ");
    }

    // funzioni per trovate Book e User dall'Id, con controlli a monte per eventuali null
    private Book GetBookById(Guid id)
    {
        return books.Find(b => b.Id == id)
            ?? throw new InvalidOperationException("Libro non trovato.");
    }
    private User GetUserById(Guid id)
    {
        return users.Find(u => u.Id == id)
            ?? throw new InvalidOperationException("Utente non trovato.");
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
        Console.WriteLine("Libro aggiunto.");
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
        Console.WriteLine("Utente aggiunto.");
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

        // Lista libri prestabili: selezione dei soli libri disponibili
        List<Book> availableBooks = books
            .Where(b => !loans.Any(l => l.BookId == b.Id && !l.IsReturned))
            .ToList();

        if (availableBooks.Count == 0)
        {
            Console.WriteLine("Nessun libro disponibile per il prestito.");
            return;
        }

        // Selezione libro
        Console.WriteLine("Seleziona libro:");
        for (int i = 0; i < availableBooks.Count; i++)
            Console.WriteLine($"{i} - {availableBooks[i].Title}");

        if (!int.TryParse(Console.ReadLine(), out int bookIndex) ||
            bookIndex < 0 || bookIndex >= availableBooks.Count)
        {
            Console.WriteLine("Selezione non valida.");
            return;
        }

        // Selezione utente
        Console.WriteLine("Seleziona utente:");
        for (int i = 0; i < users.Count; i++)
            Console.WriteLine($"{i} - {users[i].Name} {users[i].Surname}");

        if (!int.TryParse(Console.ReadLine(), out int userIndex) ||
            userIndex < 0 || userIndex >= users.Count)
        {
            Console.WriteLine("Selezione non valida.");
            return;
        }

        // Creazione prestito
        Loan loan = new Loan
        {
            BookId = availableBooks[bookIndex].Id,
            UserId = users[userIndex].Id,
            DueDate = DateTime.Now.AddDays(14)
        };

        loans.Add(loan);
        SaveData();

        Console.WriteLine("Prestito registrato.");
    }

    private void ShowLoans()
    {
        List<Loan> currentLoans = loans
            .Where(l => !l.IsReturned)
            .ToList();

        if (currentLoans.Count == 0)
        {
            Console.WriteLine("Nessun prestito presente.");
            return;
        }

        foreach (Loan l in currentLoans)
        {
            Book b = GetBookById(l.BookId);
            User u = GetUserById(l.UserId);

            Console.WriteLine($"{b.Title} → {u.Name} {u.Surname} (scadenza {l.DueDate:d})");
        }
    }

    private void ReturnLoan()
    {
        // Lista prestiti attivi
        List<Loan> currentLoans = loans
            .Where(l => !l.IsReturned)
            .ToList();

        if (currentLoans.Count == 0)
        {
            Console.WriteLine("Nessun prestito attivo al momento.");
            return;
        }

        // Selezione prestito
        Console.WriteLine("Seleziona prestito:");
        for (int i = 0; i < currentLoans.Count; i++)
        {
            Loan l = currentLoans[i];
            Book b = GetBookById(l.BookId);
            User u = GetUserById(l.UserId);

            Console.WriteLine($"{i} - {b.Title} → {u.Name} {u.Surname} (scadenza {l.DueDate:d})");
        }

        if (!int.TryParse(Console.ReadLine(), out int loanIndex) ||
            loanIndex < 0 || loanIndex >= currentLoans.Count)
        {
            Console.WriteLine("Selezione non valida.");
            return;
        }

        // Chiudi il prestito
        currentLoans[loanIndex].IsReturned = true;
        SaveData();
        Console.WriteLine("Libro restituito correttamente.");
    }

    private void ShowOverdueLoans()
    {
        List<Loan> overdueLoans = loans
            .Where(l => !l.IsReturned && l.DueDate < DateTime.Now)
            .ToList();

        if (overdueLoans.Count == 0)
        {
            Console.WriteLine("Nessun prestito scaduto.");
            return;
        }

        Console.WriteLine("Prestiti scaduti:");

        foreach (Loan l in overdueLoans)
        {
            Book b = GetBookById(l.BookId);
            User u = GetUserById(l.UserId);

            int daysLate = (DateTime.Now - l.DueDate).Days;

            Console.WriteLine
            (
                $"{b.Title} → {u.Name} {u.Surname} | " +
                $"scaduto da {daysLate} giorni (scadenza {l.DueDate:d})"
            );
        }
    }

}

