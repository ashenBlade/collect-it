import authorizedFetch from "./AuthorizedFetch";
import {serverAddress as server} from "../constants";
import Subscription from "../components/entities/subscription";
import {ResourceType} from "../components/entities/resource-type";

const baseApiPath = `${server}/api/v1/subscriptions`;

export default class SubscriptionsService {
    private static readonly fetch = authorizedFetch();

    static async getSubscriptionsPagedAsync({pageSize, pageNumber, type}: {pageSize: number, pageNumber: number, type: ResourceType}) {
        if (pageSize < 1 || pageNumber < 1) {
            throw new Error('Page size and page number must be positive');
        }
        const s = `${baseApiPath}?page_size=${pageSize}&page_number=${pageNumber}&type=${type}`;
        console.log(`Sending request to : ${s}`);
        const response = await SubscriptionsService.fetch(s, {
            method: 'GET'
        });
        if (!response.ok) {
            throw new Error('Could not get subscriptions from server');
        }
        const json = await response.json();
        const subscriptions: Subscription[] = json.subscriptions;
        const totalCount = Number(json.totalCount);
        return {
            totalCount: totalCount,
            subscriptions: subscriptions
        };
    }
}