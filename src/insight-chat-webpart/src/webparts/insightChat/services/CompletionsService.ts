import { HttpClient, IHttpClientOptions, HttpClientResponse } from '@microsoft/sp-http';
import { ICompletionsResponse } from '../models/ICompletionsResponse';

export default class CompletionsService {
    private readonly _httpClient: HttpClient;

    constructor(httpClient: HttpClient) {
        this._httpClient = httpClient;
    }

    public async getCompletions(
            query: string) : Promise<ICompletionsResponse> {

        const requestHeaders: Headers = new Headers();
        requestHeaders.append('Content-type', 'application/json');

        const body = {
            input: query
        };

        const httpClientOptions: IHttpClientOptions = {
            body: JSON.stringify(body),
            headers: requestHeaders
        };

        const response: HttpClientResponse =
            await this._httpClient.post(
                "https://localhost:8080/api/chat",
                HttpClient.configurations.v1,
                httpClientOptions);

        const completionsResponse: ICompletionsResponse = await response.json();

        return completionsResponse;
    }
}