### Day 6:電腦原理 — 網路基礎(TCP/IP、HTTP)+ HttpClient

**練習 1:概念暖身(先寫筆記再寫 code)**
用自己的話回答:
- 在瀏覽器輸入一個網址到畫面出現,中間經過哪些步驟?(DNS → TCP 連線 → TLS → HTTP 請求 → 回應 → 渲染)
    輸入網址發送到 DNS 來取得 server 真實 IP 位置, 然後建立 TCP 連線三次握手確認 client 跟 server 有正確連線
    TLS 驗證憑證 + 交換金鑰(之後所有資料都加密 —— 這才是主要目的), 然後再發出 HTTP Request 給 server 取得 response 渲染畫面

詳解: 
    ① DNS 查詢          網址 → IP(這本身也是一次網路往返)
    ② TCP 三向交握       建立連線
    ③ TLS 交握          驗證憑證 + 交換金鑰
    ④ HTTP 請求/回應     拿到 HTML
    ⑤ 解析 HTML         發現還需要 CSS/JS/圖片 → 再發更多請求(回到 ①~④)
    ⑥ 渲染              排版、繪製

- TCP 和 UDP 差在哪?各自適合什麼場景?
    	    |  TCP	                              |  UDP
送達保證	    ✅ 沒收到會重傳	                    ❌ 丟了就丟了
順序保證	    ✅ 會重新排序	                    ❌ 後發的可能先到
流量/壅塞控制	 ✅ 會依網路狀況調速	                  ❌ 沒有
開銷	        大(標頭 20+ bytes、要維護連線狀態)	  小(標頭 8 bytes)
速度	        較慢	                            快

    TCP → 網頁、檔案下載、Email、資料庫連線     「不能有一個位元組出錯」
    UDP → 影音串流、線上遊戲、DNS、VoIP        「寧可掉一格畫面,也不要卡住」
    關鍵直覺:視訊通話掉一個封包,重傳回來也沒用了(那一瞬間已經過去)。所以寧可不重傳。

- 什麼是「三向交握」?為什麼需要它?
    三向交握就是 client 跟 server 端之間的三次溝通 client 發送 server 收到, server 收到後發送給 client, client 收到後再告訴 server 收到
    需要三向交握是為了確認「雙向」都能通(我發你能收、你發我能收),
    並且交換初始序號(ISN)作為後續排序與重傳的基礎。兩次只能確認單向。

    雙方交換並確認初始序號(ISN Initial Sequence Number)。那是 TCP 用來排序封包、偵測遺失、重傳的基礎 —— 沒有序號,可靠傳輸就無從做起。

- HTTP 狀態碼的五個級距(1xx~5xx)各代表什麼?分別舉兩個常見的例子。
    1xx - 資訊性(還沒結束), 100(Continue), 101(Switching Protocols)
    2xx - 代表成功, 200(成功), 201(Created), 204(No Content 常用在 Delete 成功)
    3xx - 重新導向, 301(永久轉址), 302(暫時轉址), 304(Not Modified 快取還有效, 不用重傳)
    4xx - 用戶端錯誤, 400(Bad Request), 401(未驗證), 403(禁止), 404(找不到)
    5xx - 伺服器端錯誤, 500(Internal Error), 502(Bad Gateway), 503(服務暫停)

- HTTP/1.1、HTTP/2、HTTP/3 的主要差異是什麼?(不用深入,知道各自解決什麼問題即可)
    詳解:

    HTTP/1.1:
        核心改進: 持久連線
        解決什麼: 但一條連線一次只能處理一個請求, 前面的沒回完後面就卡住(隊頭阻塞)。瀏覽器只好開 6 條連線硬繞。

    HTTP/2:
        核心改進: 多工 + header 壓縮
        解決什麼: 一條連線可以同時跑多個請求, 不用再開 6 條。但仍受 TCP 層的隊頭阻塞(一個封包掉了, 整條連線的所有請求都要等重傳)
    
    HTTP/3:
        核心改進: 改用 QUIC(建立在 UDP 上)
        解決什麼: 解決 TCP 層的隊頭阻塞; 連線建立更快(TLS 握手合併); 換網路(Wi-Fi -> 4G) 不用重連
        
        關鍵:不是 UDP 比較快,而是 TCP 的「按序交付」寫死在作業系統裡拿不掉。
        QUIC 需要一個什麼都不管的底層,自己在上面實作「每個串流各自可靠」。
        → 串流 B 掉封包時,串流 C 照樣交付,不像 TCP 全部一起等。



**練習 3:HttpClient 的生命週期(這題是實務重點)**
- 查一下為什麼 `using var client = new HttpClient()` 是**錯誤**用法,以及 socket exhaustion 是什麼。
    因為建立後的 HttpClient 在 Dispose 後並不會馬上清除, 會以 TIME_WAIT 的狀態下存活 240 秒才被釋放,
    <!-- 錯誤回答❌ 又由於 HttpClient 沒有實作 Connection Pool, 不會重複使用建立過的連線, 
    所以有可能導致建立太多 Socket 滿了沒法再建立的 socket exhaustion 的問題 -->
    `HttpClient` 有連線池, 但是是在底層的 `HttpMessageHandler`(現在實作的是 `SocketsHttpHandler`)裡面。
    HttpClient  →  SocketsHttpHandler  →  連線池(真正持有 TCP 連線)
    問題不是沒有池子, 是每次 `new HttpClient` 都會順手 new 一個 handler, 也就是一個新的空池子。兩個 HttpClient 實例之間無法共用連線, 每個都要重付 TCP + TLS 交握成本。

- 查一下為什麼把 `HttpClient` 當成 `static` 單例長期重用,又會遇到 DNS 變更的問題。
    <!-- 錯誤回答❌ 把它當成 Singleton 的 static 實例重用就能避開建立多餘的 Socket 連線的問題, 但是他會 cache DNS 解析後的結果,
    如果哪天 DNS 改變了程式沒有重啟就會連線失敗 -->
    是因為會一直持有那條已經建立好的 TCP 連線, 所以從來沒有「需要重新解析」的時機。
    差別很微妙但重要 —— 不是「記住了舊 IP」,是「連線還活著,根本沒再查過」。所以解法才是「讓連線有壽命」(PooledConnectionLifetime),而不是「清 DNS 快取」。清快取沒用,因為它根本沒在查。
    

- 寫下結論:實務上該怎麼用?(提示:`IHttpClientFactory`,Day 8 會正式用到)
    實務上說會使用 `IHttpClientFactory` 來去做管理, 但是我有看了一下網路上教學看不太明白

    問題本質:連線要重用(省 socket)又要定期重建(更新 DNS)—— 兩個需求矛盾。
    解法:讓連線「有壽命」。
    - 有 DI 的專案(ASP.NET Core)→ IHttpClientFactory,預設 2 分鐘輪替 handler
    - 單純的 console → static HttpClient + SocketsHttpHandler.PooledConnectionLifetime
    絕對不要:每次 new 再 Dispose。

(DNS → TCP 連線 → TLS → HTTP 請求 → 回應 → 渲染)
**驗收標準**
- 能講清楚一個 HTTP 請求從發出到收到回應,中間發生了什麼(對照 Day 4 學的 I/O 機制)。
    1. 先跟 DNS 取得正確伺服器 IP
    2. 建立 TCP 連線 (三次握手)
    3. TLS 驗證憑證, 交換密鑰
    4. 發送 HTTP 請求
    5. 取得 response
    6. 前端渲染

    對照 Day 4:HTTP 請求是典型的 I/O bound —— 時間幾乎都花在等網路,CPU 閒著。
    await 在等待期間會把執行緒還回執行緒池,所以 Task.WhenAll 跑 10 個請求
    不需要 10 條執行緒,它們只是「同時在等」。
    這就是並行版本快的原因:不是運算變快,是等待時間重疊了。


- 能說出 `HttpClient` 的正確用法與兩種錯誤用法各自的後果。
    使用 IHttpClientFactory 來去左使用管理, 能夠重複使用(Connection socket 不會爆炸)也能夠定期更新(DNS 更新不會找不到)
    錯誤用法就是每次都使用 new 建立一個 client 或者是使用 (static + 預設設定) 就固定不更新了導致我上面說的問題

- 並行版本的總耗時應明顯少於依序版本(記下實際數字)。
    依序執行 1216 毫秒
    並行版本為 363 毫秒

- 能正確處理「請求失敗」與「逾時」兩種情況,程式不會崩潰。
    使用 TaskCanceledException + when e.InnerException is TimeoutException 來處理逾時狀況
    HttpRequestException 處理請求失敗

**延伸**
- 用瀏覽器 DevTools 的 Network 面板打開同一個 API,對照 C# 收到的 header 和狀態碼。
    ===== HttpClient 實際送出的 Request Headers =====
    (完全空的！一個 header 都沒加)

    使用的 HTTP 版本: HTTP/1.1
    request header 一個都沒有。 而瀏覽器打同一個網址,DevTools 會看到 User-Agent、Accept、Accept-Language、Accept-Encoding、Cookie、Sec-Fetch-*… 十幾個。

    1. 瀏覽器幫你做了超多事,HttpClient 什麼都不做
    這在打真實 API 時會咬你 —— 有些 API 看到沒有 User-Agent 會直接拒絕(403),有些需要 Accept: application/json 才回 JSON。瀏覽器測得通的 API,HttpClient 可能打不通,原因往往就在這。

    2. 回應裡藏著你筆記提到的東西
    Connection: keep-alive          ← 你量到的連線重用，就是這個在生效
    Alt-Svc: h3=":443"              ← 伺服器說「我支援 HTTP/3」
    ETag: W/"124-yiKdLzqO5gf..."    ← 搭配 If-None-Match 就能拿到你筆記寫的 304
    cf-cache-status: HIT            ← Cloudflare 快取命中
    Age: 28596                      ← 這份回應在 CDN 快取裡躺了 28596 秒
    x-ratelimit-remaining: 3599999  ← 限流額度  


    Alt-Svc: h3=":443" 特別值得注意:伺服器在宣告支援 HTTP/3,瀏覽器看到會自動升級,但你的 HttpClient 用的是 HTTP/1.1。.NET 預設不會自動升級,要自己開:
    ```csharp
    client.DefaultRequestVersion = HttpVersion.Version20;
    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher; 
    ```

- 查一下 REST 的幾個原則,以及 `GET`/`POST`/`PUT`/`PATCH`/`DELETE` 的語意差異(冪等性)——這直接銜接 Day 11 的 API 設計。
    <!-- 不精確的回答❌ PUT 不只是「修改」, 是整筆替換; PATCH 才是部分更新。
         而且完全沒答到題目特別標註的「冪等性」, 那才是這題的考點。
    `GET`: 取得查詢資料
    `POST`: 建立資料
    `PUT`: 修改資料
    `PATCH`: 不常用過忘記了
    `DELETE`: 刪除資料 -->

| 方法 | 語意 | 冪等? | 安全? |
|---|---|---|---|
| `GET` | 取得資料 | ✅ | ✅ |
| `POST` | 新增一筆 | ❌ | ❌ |
| `PUT` | **完整替換**整筆資源 | ✅ | ❌ |
| `PATCH` | **部分更新**(只送要改的欄位) | ❌ | ❌ |
| `DELETE` | 刪除 | ✅ | ❌ |

    「安全(safe)」是跟冪等性不同的另一個概念: 指完全不改變伺服器狀態。
    只有 `GET`(以及 `HEAD`、`OPTIONS`)是安全的。冪等但不安全 = 會改狀態, 但改幾次結果一樣。

`PUT` vs `PATCH` 的差別:
    PUT /posts/1          ← 整筆替換,沒給的欄位會被清掉
    { "title": "新標題", "body": "新內容", "userId": 1 }

    PATCH /posts/1        ← 只改 title,其他欄位不動
    { "title": "新標題" }

* 什麼是冪等性?
    同一個請求執行 1 次或 N 次, 最終伺服器狀態相同。
    - `GET` 冪等: 純讀取, 做幾次都不改變狀態
    - `PUT` 冪等: 「把 id=1 的 title 設成 X」 做 3 次, 結果都是 title = X
    - `POST` 不冪等: 「新增一筆」做 3 次 -> 多出 3 筆資料
    - `PATCH` 通常不冪等: 如果語意是 { "op": "increment", "value": 1 }, 做 3 次就加了 3
    - `DELETE` 冪等: 刪 3 次,結果都是「不存在」(雖然第 2、3 次回 404,但狀態沒變,冪等看的是狀態不是回應碼)

* 為什麼這題重要 —— 它直接接到練習 4 的逾時處理
    冪等性決定「請求失敗時能不能重試」。

    逾時最棘手的地方是: 你不知道伺服器到底有沒有收到並處理那個請求 ——
    可能是請求根本沒送到, 也可能是已經處理完了只是回應回不來。

    GET / PUT / DELETE → 冪等, 可以安心重試
    POST              → 不冪等, 重試可能建出兩筆重複資料!

    所以我上面寫的「逾時 → 可以重試」要加一個前提: 看是什麼 HTTP 方法。
    POST 要能安全重試, 得靠伺服器支援 idempotency key
    (客戶端自己產生一個唯一 ID 隨請求送出, 伺服器用它去重) —— 金流 API 幾乎都這樣做。

* REST 的核心原則
    1. 資源導向: URI 代表「名詞」不是「動作」。 GET /posts/1 ✅ , GET /getPost?id=1 ❌
    2. 無狀態: 每個請求自帶所有必要資訊, 伺服器不記得你上一個請求是什麼
    3. 統一介面: 用 HTTP 方法本身表達意圖, 而不是把動作塞進 URI
