// 1. 使用 TryParseTemperature 來去做 input 數字及單位的切開, 使用 out 技巧將多個值傳遞而不需要回傳複雜結構, bool function 本身回傳是否成功
bool TryParseTemperature(string? input, out double value, out char unit)
{
    value = 0;
    unit = '\0';

    if (string.IsNullOrWhiteSpace(input)) return false;

    input = input.Trim();
    unit = char.ToUpper(input[^1]);
    if (unit != 'F' && unit != 'C') return false;

    return double.TryParse(input[0..^1], out value);
}

// 2. 寫計算邏輯 function 做使用
double CelsiusToFahrenheit(double c) => c * 1.8 + 32;
double FahrenheitToCelsius(double f) => (f - 32) / 1.8;

// 3. while(true) 來去一直執行, 當輸入 exit 跳出迴圈, 輸入空值則 continue 再進入 while 一次, 使用 TryParseTemperature 來去判入輸入格式是否為 數字 + 單位(C or F)
while (true)
{
    Console.WriteLine("請輸入溫度數值與單位 (C 或 F), 或輸入 exit 離開");
    string? input = Console.ReadLine();

    if (input == "exit") break;
    if (string.IsNullOrWhiteSpace(input)) continue;
    if (!TryParseTemperature(input, out double value, out char unit))
    {
        Console.WriteLine("格式錯誤, 例如: 57F");
        continue;
    }

    // 使用計算用 function 來去做溫度的轉換
    double result = unit == 'F' ? FahrenheitToCelsius(value) : CelsiusToFahrenheit(value);
    char resultUnit = unit == 'F' ? 'C' : 'F';

    // 最後使用小技巧字串格式 result:F2 (四捨五入到小數點弟 2 位, 不會更改原值, 只有更改顯示)
    Console.WriteLine($"{value}度{unit} = {result:F2}度{resultUnit}");
}


// 1. var: 編譯時期的型別推斷。編譯器從初始值推出型別後就固定住, 之後不能變

// 2. const: 編譯時期常數: 值在編譯時就被直接嵌入到每個使用他的方法, 執行期根本不存在這個變數。
// 這帶出一個實務上的坑:如果你把 const 放在類別庫裡,改了值之後只重編譯那個類別庫、沒重編譯用它的專案,那些專案會繼續用舊的值,因為舊值早就被烤進它們的組件裡了。所以會對外發布的常數通常用 static readonly 而不是 const。

// 3. out: 不用返回複雜結構就能傳出多個值, 編譯器強制方法在返回前賦職給每個 out 參數, 所以呼叫端保證拿得到值