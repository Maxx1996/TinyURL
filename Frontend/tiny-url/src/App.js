import { useState } from "react";

function App() {
  const [URL, setURL] = useState("");
  const [TinyUrl, setTinyUrl] = useState("");
  const callApi = async () => {
    await fetch("/tinyUrl/create", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      body: JSON.stringify({
        Uri: URL,
      }),
    })
      .then((response) => response.json())
      .then((data) => setTinyUrl(data))
      .catch((error) => {
        console.error("Error:", error);
      });
    // setTinyUrl(result?.data ?? "");
    // console.log(result?.data ?? "");
  };

  return (
    <>
      <div>
        <form>
          <label>
            Enter URL:
            <input
              type="text"
              value={URL}
              onChange={(e) => setURL(e.target.value)}
            />
            <input type="button" value="Convert" onClick={callApi} />
          </label>
        </form>
      </div>
      <div>
        <h4>{TinyUrl}</h4>
      </div>
    </>
  );
}

export default App;
