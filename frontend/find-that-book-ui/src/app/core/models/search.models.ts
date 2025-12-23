export interface SearchResponse {
    query: string;
    results: BookCandidate[];
  }
  
  export interface BookCandidate {
    title: string;
    primaryAuthors: string[];
    firstPublishYear?: number;
    explanation: string;
    coverImageUrl?: string;
    workUrl: string;
  }
  
  export interface OpenLibraryMetadata {
    workUrl: string;
    coverImageUrl?: string;
  }
  