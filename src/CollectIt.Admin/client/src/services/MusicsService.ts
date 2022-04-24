import authorizedFetch from "./AuthorizedFetch";
import Image from "../components/entities/image";
import {serverAddress as server} from "../constants";
import NotFoundError from "../utils/NotFoundError";

const baseApiPath = `${server}/api/v1/musics`;

export default class MusicsService {
    private static readonly fetch = authorizedFetch();

    async getMusicsPagedAsync({pageSize, pageNumber}: {pageSize: number, pageNumber: number})
        : Promise<Image[]> {
        if (pageSize < 1 || pageNumber < 1) {
            throw new Error('Page size and page number must be positive');
        }
        const response = await MusicsService.fetch(`${baseApiPath}?page_size=${pageSize}&page_number=${pageNumber}`, {
            method: 'GET'
        });
        if (!response.ok) {
            throw new Error('Could not get images from server');
        }
        return await response.json();
    }

    async getMusicByIdAsync(id: number): Promise<Image> {
        const response = await MusicsService.fetch(`${baseApiPath}/${id}`, {
            method: 'GET'
        });
        try {
            if (!response.ok) {
                if (response.status === 404) {
                    throw new NotFoundError('Music with specified id not found');
                }
                throw new Error('Could not get music from server')
            }
            return await response.json();

        } catch (e: any) {
            throw new Error(e.message);
        }
    }
}