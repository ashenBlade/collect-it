import useAuthFetch from "./AuthorizedFetch";
import Image from "../components/entities/image";
import {serverAddress as server} from "../constants";
import NotFoundError from "../utils/NotFoundError";

const baseApiPath = `${server}/api/v1/images`;

export default class ImagesService {
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
        const response = await this.fetch(`${baseApiPath}?page_size=${pageSize}&page_number=${pageNumber}`, {
            method: 'GET'
        });
        if (!response.ok) {
            throw new Error('Could not get images from server');
        }
        return await response.json();
    }

    async getImageByIdAsync(id: number): Promise<Image> {
        const response = await this.fetch(`${baseApiPath}/${id}`, {
            method: 'GET'
        });
        try {
            if (!response.ok) {
                if (response.status === 404) {
                    throw new NotFoundError('Image with specified id not found');
                }
                throw new Error('Could not get image from server')
            }
            return await response.json();

        } catch (e: any) {
            throw new Error(e.message);
        }
    }
}