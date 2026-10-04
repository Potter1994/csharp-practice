/*
PointClass pointClass = new PointClass();
PointStruct pointStruct = new PointStruct();

// 練習 1:Value Type vs Reference Type 實驗
void ModifyClass(PointClass p) => p.X = 999;
void ModifyStruct(PointStruct p) => p.X = 999;

Console.WriteLine("==== before =====");
Console.WriteLine($"pointClass X = {pointClass.X}");
Console.WriteLine($"pointStruct X = {pointStruct.X}");

ModifyClass(pointClass);
ModifyStruct(pointStruct);

Console.WriteLine("==== after =====");
Console.WriteLine($"pointClass X = {pointClass.X}"); // pointClass.X => 999
Console.WriteLine($"pointStruct X = {pointStruct.X}"); // pointStruct.X => 0


// 練習 2:ref 關鍵字實驗 (真的傳遞變數)
void ModifyStructByRef(ref PointStruct p) => p.X = 999;
ModifyStructByRef(ref pointStruct);

Console.WriteLine("==== 對 struct 使用 ref ====");
Console.WriteLine($"pointStruct X = {pointStruct.X}");
*/

/* 練習 3:Boxing/Unboxing 實驗
int number = 42;
object boxed = number;
boxed = 100;
Console.WriteLine(number); // 42
Console.WriteLine(boxed); // 100
*/

/* 練習 4:記憶體用量觀察 
這是使用 class 去建立然後算, 這個範例 class 佔了 53MB
long before = GC.GetTotalMemory(true);
PointClass[] items = new PointClass[1_000_000];

for (int i = 0; i < items.Length; i++)
{
    items[i] = new PointClass();
}

long after = GC.GetTotalMemory(true);

Console.WriteLine($"class: {(after - before) / 1024 / 1024} MB");
Console.WriteLine(items.Length); // <- 用一下陣列, 確保他不會被提早回收
*/

/*
這邊是使用 struct 去建立然後算,  這個範例 struct 佔了 7MB
long before = GC.GetTotalMemory(true);
PointStruct[] items = new PointStruct[1_000_000];
long after = GC.GetTotalMemory(true);

Console.WriteLine($"struct: {(after - before) / 1024 / 1024} MB");
Console.WriteLine(items.Length);
*/