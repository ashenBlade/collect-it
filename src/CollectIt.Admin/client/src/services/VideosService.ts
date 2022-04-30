import authorizedFetch from "./AuthorizedFetch";
import {serverAddress} from "../constants";
import Video from "../components/entities/video";
import NotFoundError from "../utils/NotFoundError";

const baseApiPath = `${serverAddress}/api/v1/videos`;

export default class VideosService {
    private static readonly fetch = authorizedFetch();

    static async getVideosPagedAsync({pageSize, pageNumber}: {pageSize: number, pageNumber: number}){
        if (pageSize < 1 || pageNumber < 1) {
            throw new Error('Page size and page number must be positive');
        }
        const response = await VideosService.fetch(`${baseApiPath}?page_number=${pageNumber}&page_size=${pageSize}`, {
            method: 'GET'
        });
        if (!response.ok) {
            throw new Error('Could not get videos from server');
        }
        const result = await response.json();
        const videos: Video[] = result.videos;
        const totalCount: number = Number(result.totalCount);
        return { videos, totalCount };
    }

    static async getVideoByIdAsync(id: number): Promise<Video> {
        const response = await VideosService.fetch(`${baseApiPath}/${id}`, {
            method: 'GET'
        });
        try {
            if (!response.ok) {
                if (response.status === 404) {
                    throw new NotFoundError('Video with specified id not found');
                }
                throw new Error('Could not get video from server')
            }
            return await response.json();

        } catch (e: any) {
            throw new Error(e.message);
        }
    }


    static async changeVideoNameAsync(id: number, name: string) {
        if (!name) {
            throw new Error('No name provided');
        }

        const response = await VideosService.fetch(`${baseApiPath}/${id}/name`, {
            method: 'POST',
            body: JSON.stringify({
                name: name
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            console.error(`Could not change video name. Server status: ${response.status}`);
            throw new Error('Could not change video name');
        }
    }

    static async changeVideoTagsAsync(id: number, tags: string[]) {
        if (!tags) {
            throw new Error('Tags can not be null or undefined');
        }

        const response = await VideosService.fetch(`${baseApiPath}/${id}/tags`, {
            method: 'POST',
            body: JSON.stringify({
                tags: tags
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            console.error(`Could not change video tags. Server status: ${response.status}`);
            throw new Error('Could not change tags');
        }
    }

    static async deleteVideoByIdAsync(id: number) {
        const response = await VideosService.fetch(`${baseApiPath}/${id}`, {
            method: 'DELETE'
        });
        if (!response.ok) {
            if (response.status === 404) throw new NotFoundError('No video found')
            throw new Error('Could not delete video');
        }
    }
}