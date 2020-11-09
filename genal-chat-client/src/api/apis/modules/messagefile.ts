import fetch from '@/api/fetch';

/**
 * 群名模糊搜索用户
 * @param string
 */
export function sendImageMessage(params: FormData) {
    return fetch.post(`/upload/SendImageMessage`,params, {
        headers: {
          'Content-Type': 'multipart/form-data;',
        },
      });
}
