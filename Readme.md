Based on the project requirements you shared, your **README** should clearly explain how to set up, run, and understand your library discovery application. Here’s a detailed structure with all necessary sections:

---

## **1. Project Overview**

* A brief description of the project:

  > "Find That Book" is a full-stack application that helps users find books based on messy, partial, or noisy queries. It uses AI (Gemini) for query extraction and Open Library APIs to fetch book details, providing explanations for each candidate.

* Mention the tech stack:

  * **Backend:** .NET 8 Web API
  * **Frontend:** Angular 21
  * **LLM Integration:** Google Gemini API
  * **External API:** Open Library

---

## **2. Features Implemented**

* Accepts messy queries (author, title, or keywords).
* Uses AI to extract structured information: `{title, author, keywords}`.
* Searches Open Library for candidates.
* Ranks candidates using title + author + contributor hierarchy.
* Generates short, one-sentence explanations per candidate.
* Returns top 5 matches with metadata:

  * Title
  * Primary authors
  * First publish year
  * Open Library link
  * Cover image (if available)
  * Explanation

---

## **3. Setup Instructions**

### **Backend (API)**

1. Navigate to the backend folder:

   ```bash
   cd FindThatBook.API
   ```
2. Restore NuGet packages:

   ```bash
   dotnet restore
   ```
3. Run the API:

   ```bash
   dotnet run
   ```
4. By default, the API runs on `https://localhost:7243` (HTTPS only).
5. Configure Gemini API key in `appsettings.json`:

   ```json
   "Gemini": {
     "ApiKey": "<YOUR_API_KEY>"
   }
   ```

### **Frontend (Angular)**

1. Navigate to the frontend folder:

   ```bash
   cd FindThatBook.UI
   ```
2. Install dependencies:

   ```bash
   npm install
   ```
3. Run the development server:

   ```bash
   ng serve
   ```
4. Front end runs on `http://localhost:4200`.

> Make sure the API URL in `search.service.ts` points to `https://localhost:7243/api`.

---

## **7. AI API Setup**

* Include your API key in `appsettings.json` (backend).
---

## **8. Run the Application**

1. Start backend first (`dotnet run`).
2. Start frontend (`ng serve`).
3. Open `http://localhost:4200` in your browser and test search queries.

