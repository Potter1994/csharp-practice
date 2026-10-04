Library libraryA = new Library(500, "A Library");
Library libraryB = new Library(2000, "B Library");
Library libraryC = new Library(1500, "C Library");

Book book = new Book("Harry Potter", "Kevin", "isbn");
Book book1 = new Book("Harry", "Kevin", "isbn");
Magazine magazine = new Magazine("Jay Magazine", 55688);
libraryA.AddLibraryItem(book);
libraryA.AddLibraryItem(book1);
libraryA.AddLibraryItem(magazine);

libraryB.AddLibraryItem(book);

libraryC.AddLibraryItem(book1);

Member member = new Member("Potter");
Member member1 = new Member("Andy");
libraryA.AddMember(member);
libraryA.AddMember(member1);

libraryA.Borrow(member, book);
libraryA.Borrow(member, book1);
libraryA.Borrow(member1, magazine);

libraryA.GetMostBookMember();
Console.WriteLine(libraryA.GetDueDateBook());
libraryA.GetCountGroupByType();
libraryA.SearchBorrowedListByMember(member);
libraryA.CheckItem(book);
// libraryA.Return(book);
// libraryA.SearchBorrowedListByMember(member);
// libraryA.CheckItem(book);
// libraryA.Borrow(member1, book1);
// Console.WriteLine(book.Title);

// IRenewable r = book;

// book.Borrow(member);
// Console.WriteLine($"到期日: {book.DueDate:yyyy-MM-dd}");

// book.Renew();
// Console.WriteLine($"續借後: {book.DueDate:yyyy-MM-dd}");    // 應該再 +30 天

// book.Return();
// Console.WriteLine($"歸還後: {book.DueDate?.ToString("yyyy-MM-dd") ?? "無"}");  // 應該是「無」
