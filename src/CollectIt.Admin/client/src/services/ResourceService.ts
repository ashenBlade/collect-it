import {serverAddress as server} from "../constants";
import useAuthFetch from "./AuthorizedFetch";
import {ResourceType} from "../components/entities/resource-type";
import Image from "../components/entities/image";

export class ResourceService {
    private readonly fetch: (info: RequestInfo,
                             init: RequestInit | null) => Promise<Response>;
    constructor() {
        this.fetch = useAuthFetch();
    }
    async getImagesPagedAsync({pageSize, pageNumber}: {pageSize: number, pageNumber: number})
        : Promise<Image[]> {
        if (pageSize < 1 || pageNumber < 1) {
            throw new Error('Page size and page number must be positive');
        }
        const response = await this.fetch(`${server}/api/v1/images`, {
            body: JSON.stringify({
                pageNumber: pageNumber,
                pageSize: pageSize,
                type: ResourceType.Image
            }),
            method: 'GET'
        });
        if (!response.ok) {
            throw new Error('Could not get images from server');
        }
        return await response.json();
    }
}