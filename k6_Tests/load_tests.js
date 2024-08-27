import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '30s', target: 50 },  // Ramp-up to 50 users over 30 seconds
        { duration: '1m', target: 50 },   // Stay at 50 users for 1 minute
        { duration: '10s', target: 200 }, // Spike to 200 users over 10 seconds
        { duration: '1m', target: 200 },  // Stay at 200 users for 1 minute
        { duration: '10s', target: 50 },  // Ramp-down to 50 users over 10 seconds
        { duration: '30s', target: 0 },   // Ramp-down to 0 users over 30 seconds
    ],
};

const BASE_URL = 'http://localhost:3000/api';
const TOKEN = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJnaXF1YXJkbyIsImVtYWlsIjoiZ2lxdWFyZG9AZ21haWwuY29tIiwiSXNBZG1pbiI6IlRydWUiLCJVc2VySWQiOiIxIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJleHAiOjE3MjQ3NzI3NzcsImlzcyI6IlBob3RvQWxidW1BcGkiLCJhdWQiOiJQaG90b0FsYnVtQXBpVXNlcnMifQ.U8_XTw4zK20lz9HYPJ0_2yMbJ0MJ05LAeHRqBl0wVEE'; // Replace with the obtained JWT token

export default function () {
    let params = {
        headers: {
            'Authorization': `Bearer ${TOKEN}`,
            'Content-Type': 'application/json'
        },
    };

    // Placeholder IDs for endpoints requiring an ID
    const albumId = '1'; // Replace with actual album ID
    const photoId = '1'; // Replace with actual photo ID

    // Perform multiple GET requests
    let responses = http.batch([
        ['GET', `${BASE_URL}/v1.0/albums`, null, params],
        ['GET', `${BASE_URL}/v1.0/albums/${albumId}`, null, params],
        ['GET', `${BASE_URL}/photos`, null, params],
        ['GET', `${BASE_URL}/photos/${photoId}`, null, params]
    ]);

    // Check responses
    check(responses[0], {
        'albums status was 200': (r) => r.status === 200,
    });
    check(responses[1], {
        'album by id status was 200': (r) => r.status === 200,
    });
    check(responses[2], {
        'photos status was 200': (r) => r.status === 200,
    });
    check(responses[3], {
        'photo by id status was 200': (r) => r.status === 200,
    });

    sleep(1);
}