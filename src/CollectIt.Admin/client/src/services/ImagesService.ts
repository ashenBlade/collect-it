import Image from "../components/entities/image";
import {serverAddress as server} from "../constants";
import NotFoundError from "../utils/NotFoundError";
import authorizedFetch from "./AuthorizedFetch";

const baseApiPath = `${server}/api/v1/images`;

export default class ImagesService {
    private static readonly fetch = authorizedFetch();

    static async getImagesPagedAsync({pageSize, pageNumber}: {pageSize: number, pageNumber: number}){
        if (pageSize < 1 || pageNumber < 1) {
            throw new Error('Page size and page number must be positive');
        }
        const response = await ImagesService.fetch(`${baseApiPath}?page_size=${pageSize}&page_number=${pageNumber}`, {
            method: 'GET'
        });
        if (!response.ok) {
            throw new Error('Could not get images from server');
        }
        const result = await response.json();
        const total = Number(result.totalCount);
        const images: Image[] = result.images;
        return {
            totalCount: total,
            images: images
        };
    }

    static async getImageByIdAsync(id: number): Promise<Image> {
        const response = await ImagesService.fetch(`${baseApiPath}/${id}`, {
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

    static async changeImageNameAsync(id: number, name: string) {
        if (!name) {
            throw new Error('No name provided');
        }

        const response = await ImagesService.fetch(`${baseApiPath}/${id}/name`, {
            method: 'POST',
            body: JSON.stringify({
                name: name
            })
        });
        if (!response.ok) {
            console.error(`Could not change image name. Server status: ${response.status}`);
            throw new Error('Could not change image name');
        }
    }

    static async changeImageTagsAsync(id: number, tags: string[]) {
        if (!tags) {
            throw new Error('Tags can not be null or undefined');
        }

        const response = await ImagesService.fetch(`${baseApiPath}/${id}/tags`, {
            method: 'POST',
            body: JSON.stringify({
                tags: tags
            })
        });
        if (!response.ok) {
            console.error(`Could not change image tags. Server status: ${response.status}`);
            throw new Error('Could not change tags');
        }
    }

    static async deleteImageByIdAsync(id: number) {
        const response = await ImagesService.fetch(`${baseApiPath}/${id}`, {
            method: 'DELETE'
        });
        if (!response.ok) {
            if (response.status === 404) throw new NotFoundError('No music found')
            throw new Error('Could not delete image');
        }
    }
}