**練習 1:概念暖身(先寫筆記再動手)**
- **Authentication 跟 Authorization 差在哪?** 對應的 HTTP 狀態碼分別是什麼?(提示:401 和 403 的語意完全不同,而 401 的名字其實取錯了)
    Authentication: 身份驗證, 確認你是誰, 對應 401 Unauthorized(代表未驗證或驗證失敗)
    Authorization: 身份的權限驗證, 確認你有沒有這個權限, 對應 403 Forbidden

- **JWT 的三個部分**是什麼?為什麼用 `.` 分隔?去 jwt.io 貼一個你等下會產生的 token 進去看。
    三段都用 Base64Url 編碼,而 Base64Url 的字元集是 A-Z、a-z、0-9、-、_(標準 Base64 的 + / 換成 - _,= padding 去掉)。. 不在這個字元集裡 —— 所以它不可能出現在任何一段的內容中,拿來當分隔符絕對不會跟資料混淆。
    1. Header (標頭):
        - 功能: 描述該 Token 的基本中繼資料(Metadata)
        - 內容: 權杖類型(如 `JWT`) 與簽章演算法名稱(`alg`: 如 `HS256`或`RS256`)
        - 編碼方式: Base64Url 編碼
    2. Payload (有效負載):
        - 功能: 存放實際要傳遞的資料與聲明(Claims)
        - 內容: 包含標準欄位(如 `sub` 主題、`exp` 過期時間、`iat` 發行時間) 或自訂業務資料
        - 安全注意: 僅使用 Base64Url 編碼、並未加密，任何人皆可解碼讀取內容，勿放入密碼等敏感機密
    3. Signature (簽章):
        - 功能: 用來驗證資料的完整性，確保內容在傳遞中沒有被竄改
        - 產生方式: 將編碼後的 `Header` 與 `Payload` 加上密鑰(Secret Key)，再透過 `Header` 指定的演算法進行雜湊運算產生

- **JWT 不是加密,是簽章。** 把 payload 那段單獨拿出來 base64url 解碼,確認你能讀到裡面的內容。然後回答:**什麼東西絕對不能放進 JWT?**
    JWT 只是使用 Base64Url 編碼, 任何人都可以解碼讀取內容, 勿放入密碼等敏感資料
    什麼東西不能放:
    1. 看了就有價值的資料: 密碼(當然)、身分證號、信用卡、健康紀錄、其他系統的 API key、內部定價。判斷標準很簡單:假設這段 JSON 會被貼到公開的 Pastebin,你介意嗎?
    2. 會變動的授權資料: 把 `role: "admin"` 塞進 JWT,那在 token 過期前把這個人降級就是無效的。你題 4 寫的「停權在 2 小時內不生效」和這件事是同一個機制:凡是放進 token 的東西,都被凍結到過期為止。所以判斷標準是:這個值在 token 有效期內可能改變嗎?如果會,改了之後還依賴舊值會不會造成損害?
    3. 太大的東西 —— JWT 每一個請求都會帶在 header 裡。塞進去的每個欄位都要乘以你的 QPS。而且很多 proxy 和伺服器對 header 總大小有上限(nginx 預設 8KB),超過會回 431 或直接斷線 —— 而且是在你的程式碼之前就被擋掉,日誌裡什麼都看不到。把使用者的完整權限清單放進 JWT 的系統,常常就是這樣爆的。

- 有了 JWT 為什麼就不需要伺服器端的 session?**代價是什麼?**(提示:想一下「使用者被停權」這件事多快會生效)
    JWT 採用無狀態(Stateless) 與「自包含(Self-contained)」的設計
    簡單來說，JWT 把使用者的身份資料、權限和過期時間，全部簽章後「直接交給瀏覽器/客戶端儲存」。每次客戶端發送請求時，只要帶著這個 Token 伺服器只需要用密鑰驗證簽名是否正確，就能確認這個 token 是自己發的、沒被篡改，不需要再去資料庫貨 Redis 查閱 Session。

    代價是: 喪失即時控制權，還有「你不知道誰在線上」
    當你放棄了中央控管的 Session，最核心的代價就是伺服器失去了對憑證的「絕對控制權」
    以「使用者被停權」為例:
    - 傳統 Session 作法:
        管理員在後台點擊「停權」，伺服器直接從 Redis/資料庫刪除該使用者的 Session ID。
        使用者下一次點擊頁面時，伺服器查不到 Session，立刻強制登出，即時生效(毫秒級)。
    - JWT 的代價:
        因為伺服器不保存狀態，只要 JWT 還在有效期限內(例如還有 2 小時)，即便管理員在後
        台將該使用者停權或修改了密碼，該 JWT 依然合法。駭客或被停權的使用者持有這個 Token
        ，在未過期的 2 小時內仍然可以暢通無阻。

- **為什麼 JWT 很難「登出」?** 業界怎麼處理這個問題?
    JWT 本身不具備讓簽發者主動撤銷 Token 的機制，客戶端頂多只是把瀏覽器的 JWT 刪除。但如果這個 JWT 被駭客
    竊取，駭客在 Token 過期前依然可以繼續冒用。
    - 黑名單機制(Blacklist):
        當使用者登出或被停權時，將該 JWT 的唯一識別碼 (jti) 丟進 Redis 的黑名單，並設定與 JWT 相同的過期時間。
        伺服器收到請求時，先查一下黑名單。(缺點: 讓伺服器又變回微有狀態了，失去了部分純 JWT 的效能優勢)
    - 長短 Token 搭配(Access / Refresh Token):
        - Access Token (JWT): 有效期限極短 (例如 10-15 分鐘)，用來應付頻繁的 API 請求。被停權時，最多只會漏勾這 10 幾分鐘。
        - Refresh Token: 有效時間較長 (例如 7 天)，儲存在伺服器端資料庫。當 Access Token 過期時，客戶端必須拿 Refresh Token 去換新的。
            此時伺服器就能可以檢查「使用者是否已經被停權」，如果是，就拒絕核發新 Token

- 密碼為什麼不能明文存?**雜湊跟加密差在哪**(哪一個是單向的)?為什麼不能用 MD5 或 SHA256 來雜湊密碼?(提示:查 "password hashing" 跟 "salt"、"work factor")
    1. 為什麼密碼不能明文儲存:
        密碼明文存有被駭客竊取的風險，即使資料庫遭到竊取，系統也不應該讓駭客輕易還原使用者的密碼。

    2. 雜湊 (Hashing) 跟加密 (Encryption) 差在哪?
        - 加密 Encryption: 加密的目的，是保護資料，同時保留還原資料的能力。持有正確金鑰的人，可以將密文解密回原始明文。
        - 雜湊 Hashing: 雜湊的設計目標是單向運算，可以從輸入計算雜湊值，但沒有一般可行方法直接或從雜湊值反推出原始輸入。

        最簡單的記法:
        - 加密: 可以透過金鑰解密還原。
        - 雜湊: 沒有用來還原原文的解密程序。
        - 密碼驗證: 通常不需要還原密碼，只要確認使用者輸入的密碼是否正確即可。
    注意: 雜湊並不是絕對無法猜回原文。攻擊者可以猜測可能的密碼，再計算雜湊值比對。因此，接下來才要討論為什麼不能直接使用 MD5 或 SHA-256。

    3. 為什麼不能直接使用 MD5 或 SHA-256 ?
    關鍵不是他們不安全，而是: 他們的設計目標是快速計算雜湊值，而不是讓密碼破解變得昂貴。
    假設使用者的密碼是 123456: 駭客猜測密碼 123456 -> 計算 `SHA256("123456")` -> 與資料庫雜湊值做比對 -> 一致(猜對密碼)
    整個過程不需要向你的 API 發送請求，因此也不會受到你網站的登入限流機制保護。
    這就是離線暴力破解 (Offline Brute Force)
    SHA-256 本身仍然是廣泛使用的安全雜湊函數，問題在於他算得太快。駭客可以使用 GPU 大量嘗試候選密碼。
    所以密碼儲存需要不是單純「安全的雜湊函數」，而是專門為密碼設計、可以調整計算成本的密碼雜湊演算法。

    4. Salt (鹽值) 是什麼?
    Salt 是一段隨機產生的資料，會在計算密碼雜湊時加入，且每個使用者應該擁有獨立的 Salt。
    - 避免相同密碼產生相同的雜湊結果，降低攻擊者從大量雜湊值辨識共用密碼的能力。
    - 讓預先計算的雜湊查表 (例如 Rainbow Table) 難以直接使用，因為每個 Salt 要個別計算。

    Salt 通常會跟雜湊值一起儲存在資料庫中。
    它不是用來隱藏密碼，而是讓每一筆密碼雜湊具有不同的計算輸入。

    5. Work Factor (工作因子) 是什麼?
    這是整個密碼儲存設計裡最重要的概念之一。
    Work Factor 的概念是: 刻意提高每次驗證密碼所需的成本，讓駭客每猜一次密碼都必須付出更多時間與資源。

    6. 業界實際使用什麼演算法？
    依照 OWASP 的密碼儲存建議，新系統可優先考慮 Argon2id；如果環境或合規要求不適合，也可以考慮 scrypt 或 PBKDF2。bcrypt 則常見於既有系統。

    演算法              特點
    Argon2id           現代密碼雜湊首選之一，可調整記憶體與運算成本
    scrypt             具記憶體成本設計
    PBKDF2             支援廣泛，常用於有特定合規需求的環境
    bcrypt             歷史悠久，許多既有系統仍在使用
    SHA-256 / MD5      不應直接用於密碼儲存

**練習 2:使用者與密碼雜湊**
- **Email 必須唯一** —— 這題直接用 Day 11 練習 1 題 3 的結論:唯一索引 + 應用層預檢 + 捕捉唯一約束違反例外,三層各自負責什麼?
    - 唯一索引: 在 database 層, 它不依賴任何程式碼路徑 (唯一真正保證正確性的那一層)
    - 應用層預檢: 在 Service 先用 SELECT 查一次 email 在不在, 在就不送 INSERT
        - 絕大多數請求是正常註冊,而預檢讓那條路慢了一倍。 所以第二層的價值不是效能 —— 它省掉的是「失敗的 INSERT」,而那本來就是少數情況。
        - 預檢不是為了正確性(索引負責),也不是為了效能(它反而讓正常路徑多一次往返)。它的價值是讓「哪個欄位重複」這件事在應用層就能判斷,不用依賴資料庫的索引名字 —— 等有第二個唯一約束時,這個差別會很明顯。
    - 捕捉唯一約束違反例外: 這個在 Repository 層, 主要是防止 race condition 的問題, 有可能有競態

    如果唯一索引不存在 → 資料庫不會拒絕 → 捕捉唯一約束違反例外的 catch 永遠不會觸發,重複資料直接寫進去
    捕捉唯一約束違反例外不存在 → 資料庫照樣拒絕,只是變成 500
    換句話說:第一層和第三層不是兩道獨立的防線。第三層是第一層的「翻譯器」 —— 把資料庫的 23505 翻譯成使用者看得懂的 409。

    拿掉哪一層	    壞掉什麼
    唯一索引	    正確性 —— 資料庫真的會有兩筆相同 email,而且另外兩層也跟著失效
    應用層預檢	    幾乎沒有 —— 正常路徑反而變快,重複 email 走第三層照樣回 409
    捕捉例外	    使用者體驗 —— 競態時 409 變成 500

- DTO 設計:`RegisterRequest` 該有哪些欄位?**絕對不能有什麼?**(回去看 Day 11 的 over-posting)
    `RegisterRequest` 該有 email 跟 password 欄位, 絕對不能有 id

- 註冊成功該回什麼?要不要直接回 token?(兩種做法都有人用,想清楚差別)
    註冊成功可以回成功查詢的路徑。可以直接回 token 就直接登入了


**練習 3:登入與發 JWT**
- **claims 要放什麼?** 至少需要能識別使用者的那個。查 `ClaimTypes.NameIdentifier` 跟 JWT 標準的 `sub` 有什麼關係(這裡有個 .NET 特有的坑)。
    這邊是使用 JWT 所以必要放 `sub`
    而 `ClaimTypes.NameIdentifier` 跟 `sub` 的關係
    兩者是不同的名稱
    ```csharp
        ClaimTypes.NameIdentifier
        實際值: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier

        而
        JwtRegisteredClaimNames.Sub
        實際值: sub
    ```
    但是 JwtSecurityTokenHandler 預設有 Claim Mapping。
    簡單來說就是驗證 JWT 後(使用預設 Mapping) 導致他會去找 ClaimTypes.NameIdentifier 而不是 JWT 的 sub
    這是因為 ASP.NET Core 驗證 JWT 時,預設會把 sub 映射成 ClaimTypes.NameIdentifier
    必須在 Program.cs 中新增
    ```csharp
        builder.Services
        .AddAuthentication()
        .AddJwtBearer(options =>
        {
            // 這邊告訴 ASP.NET Core: 驗證 JWT 時，不要自動將 JWT Claim 名稱轉換成 .NET 的 Claim 名稱
            options.MapInboundClaims = false;
        });
    ```

- 在 `Program.cs` 設定 `AddAuthentication().AddJwtBearer(...)`,把 `Issuer`、`Audience`、`IssuerSigningKey`、`ClockSkew` 都設明確。想一下 `ValidateIssuer` / `ValidateAudience` 各自在防什麼。
    `ValidateIssuer`: 防止金鑰外流或被共用給別的簽發者之後, 擋掉非你簽發的 token
    `ValidateAudience`: 防止為服務 A 簽的 token 被拿到服務 B 使用(兩個服務共用金鑰, A 的 token 在 B 也是合法簽章)

- **登入失敗要回什麼?** 401 還是 400?而且訊息**不能說「密碼錯誤」或「帳號不存在」** —— 為什麼?
    我覺得應該是要回傳 401 登入錯誤, 然後不能說密碼錯誤或帳號不存在是為了不讓攻擊者知道哪些 email 有註冊

枚舉為什麼有價值?拿到一份「確認存在」的帳號清單之後,攻擊者可以:撒網釣魚、拿外洩密碼庫去撞庫、或單純確認某個人有用你的服務(隱私問題)。他完全不需要破解任何密碼。
而這個混淆正是你 FakePassword 失敗的原因
你當時說「只是為了讓他消耗駭客成本隨便要讓他跑一次 verify 浪費時間」—— 那是在追求成本。但真正的目標是不可區分。
結果就是:那個明文假雜湊讓 verify 根本沒跑(所以沒浪費任何時間),還附帶一個 500(所以區分度更高了)。目標設錯,手段就偏了。


**練習 4:保護端點**

- 加 `app.UseAuthentication()` 和 `app.UseAuthorization()`。**順序很重要** —— 用 Day 11 學的那套(`Use` 之間有先後、`Map` 位置無關)推一下它們該放哪,以及為什麼 `UseAuthentication` 必須在 `UseAuthorization` 前面。
`UseAuthentication`: 處理驗證, 成功填入 `HttpContext.User`, 失敗就不填然後繼續往下走
`UseAuthorization`: 讀端點上的授權 metadata 並執行, 沒過就 401(未驗證)或 403(已驗證但沒資格)

- 四種失敗情況各測一次,記下**狀態碼和回應格式**:
四個狀態碼都是 401
四個的 body 完全相同

401 → 有 WWW-Authenticate
403 → 沒有 WWW-Authenticate(RFC 9110:重試也沒用,不需要告訴你怎麼驗證)

/*
fetch("http://localhost:5020/todos", {
    method: "GET",
    headers: {
        "Content-Type": "application/json",
    },
}).then(res => console.log(res.headers.get("www-authenticate")));
他這是要取得回應 header 的 www-authenticate 回應
*/

  - 完全沒帶 token
  Bearer

  - token 格式錯(隨便一串字)
  Bearer error="invalid_token"
﻿
  - 簽章對不上(把 key 改掉再驗)
  Bearer error="invalid_token", error_description="The signature key was not found"

  - token 過期(把 `exp` 設成 1 分鐘,等它過期)
  Bearer error="invalid_token", error_description="The token expired at '11/19/2026 07:50:39'"

- **401 跟 403 什麼時候各自出現?** 自己製造一次 403 看看。
  401 就是在有 RequireAuthorization(); 的端點, 然後沒有正確的 token 時候會出現
  403 則是使用 RequireAuthorization(p => p.RequireClaim("email", "test1@gmail.com")); // 這邊我沒有 role 所以使用 email 來去測試(再依照你真實所需要的 ClaimTypes 去做)


- 這些 401 有進到你 Day 11 做的 ProblemDetails 格式嗎?**為什麼?**(提示:回去看「五種錯誤只有一種是例外」那張表)
  有 —— 但這是因為我把 app.UseAuthorization() 明確寫在 UseStatusCodePages 的內層。
  UseStatusCodePages 只檢查「往外走的回應」,所以它只補得到比它內層產生的狀態碼。我把那行註解掉時,WebApplication 會自動補一個 UseAuthorization,但位置在它外層 → 401 的 body 就是空的。
