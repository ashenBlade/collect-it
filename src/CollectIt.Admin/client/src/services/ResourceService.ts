import {serverAddress as server} from "../constants";
import useAuthFetch from "./AuthorizedFetch";
import {ResourceType} from "../components/entities/resource-type";
import Image from "../components/entities/image";
import Video from "../components/entities/video";
import Music from "../components/entities/music";

export class ResourceService {
    private readonly fetch: (info: RequestInfo,
                             init: RequestInit | null) => Promise<Response>;
    constructor() {
        this.fetch = useAuthFetch();
    }
    async getImagesPagedAsync({pageSize, pageNumber}: {pageSize: number, pageNumber: number})
        : Promise<Image[]> {
        ResourceService.assertPositivePageSettings(pageSize, pageNumber);
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

    private static assertPositivePageSettings(pageSize: number, pageNumber: number) {
        if (pageSize < 1 || pageNumber < 1) {
            throw new Error('Page size and page number must be positive');
        }
    }

    async getVideosPagedAsync({pageSize, pageNumber}: {pageSize: number, pageNumber: number})
        : Promise<Video[]> {
        ResourceService.assertPositivePageSettings(pageSize, pageNumber);
        const response = await this.fetch(`${server}/api/v1/videos`, {
            body: JSON.stringify({
                pageNumber: pageNumber,
                pageSize: pageSize,
                type: ResourceType.Video
            }),
            method: 'GET'
        });
        if (!response.ok) {
            throw new Error('Could not get videos from server');
        }
        return await response.json();
    }

    async getMusicPagedAsync({pageSize, pageNumber}: {pageSize: number, pageNumber: number})
        : Promise<Music[]> {
        ResourceService.assertPositivePageSettings(pageSize, pageNumber);
        const response = await this.fetch(`${server}/api/v1/music`, {
            body: JSON.stringify({
                pageNumber: pageNumber,
                pageSize: pageSize,
                type: ResourceType.Music
            }),
            method: 'GET'
        });
        if (!response.ok) {
            throw new Error('Could not get music from server');
        }
        return await response.json();
    }
}