import authorizedFetch from "./AuthorizedFetch";
import {serverAddress} from "../constants";
import Video from "../components/entities/video";
import Image from "../components/entities/image";
import NotFoundError from "../utils/NotFoundError";

const baseApiPath = `${serverAddress}/api/v1/videos`;

export default class VideosService {
    private static readonly fetch = authorizedFetch();

    static async getVideosPagedAsync({pageSize, pageNumber}: {pageSize: number, pageNumber: number})
        : Promise<Video[]> {
        if (pageSize < 1 || pageNumber < 1) {
            throw new Error('Page size and page number must be positive');
        }
        const response = await VideosService.fetch(`${baseApiPath}?page_number=${pageNumber}&page_size=${pageSize}`, {
            method: 'GET'
        });
        if (!response.ok) {
            throw new Error('Could not get videos from server');
        }
        return await response.json();
    }

    static async getVideoByIdAsync(id: number): Promise<Image> {
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
}