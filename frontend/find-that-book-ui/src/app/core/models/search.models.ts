export interface SearchResponse {
    query: string;
    results: BookCandidate[];
  }
  
  export interface BookCandidate {
    title: string;
    authors?: string[];
    first_publish_year?: number;
    explanation: string;
    coverImageUrl?: string;
    workUrl: string;
  }
  
  export interface OpenLibraryMetadata {
    workUrl: string;
    coverImageUrl?: string;
  }
  