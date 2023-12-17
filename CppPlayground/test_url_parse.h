#pragma once

#define HAVE___BUILTIN_EXPECT 1
# define __builtin_expect(e, c) (e)
# define unlikely(p)   __builtin_expect(!!(p), 0)

#ifdef HAVE_CONFIG_H
# include "config.h"
#endif

#include <errno.h>
#include <limits.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>
#ifdef _WIN32
# include <io.h>
#endif

#include <ctype.h>

static const char urihex[] = "0123456789ABCDEF";

struct vlc_url_t
{
    char* psz_protocol;
    char* psz_username;
    char* psz_password;
    char* psz_host;
    unsigned i_port;
    char* psz_path;
    char* psz_option;

    char* psz_buffer; /* to be freed */
    char* psz_pathbuffer; /* to be freed */
};

/* Network */
typedef struct vlc_url_t vlc_url_t;

void smb2_parse_url(const char* url);
char* vlc_iri2uri(const char* str);
static int vlc_UrlParseInner(vlc_url_t* url, const char* str);

static int test_func(int a, int b)
{
    return a + b;
}

static int vlc_UrlParseInner(vlc_url_t* url, const char* str)
{
    url->psz_protocol = NULL;
    url->psz_username = NULL;
    url->psz_password = NULL;
    url->psz_host = NULL;
    url->i_port = 0;
    url->psz_path = NULL;
    url->psz_option = NULL;
    url->psz_buffer = NULL;
    url->psz_pathbuffer = NULL;

    if (str == NULL)
    {
        errno = EINVAL;
        return -1;
    }

    char* buf = vlc_iri2uri(str);
    if (unlikely(buf == NULL))
        return -1;
    url->psz_buffer = buf;

    char* cur = buf, * next;
    int ret = 0;

    /* URI scheme */
    next = buf;
    while ((*next >= 'A' && *next <= 'Z') || (*next >= 'a' && *next <= 'z')
        || (*next >= '0' && *next <= '9') || memchr("+-.", *next, 3) != NULL)
        next++;

    if (*next == ':')
    {
        *(next++) = '\0';
        url->psz_protocol = cur;
        cur = next;
    }

    /* Fragment */
    next = strchr(cur, '#');
    if (next != NULL)
    {
#if 0  /* TODO */
        * (next++) = '\0';
        url->psz_fragment = next;
#else
        * next = '\0';
#endif
    }

    /* Query parameters */
    next = strchr(cur, '?');
    if (next != NULL)
    {
        *(next++) = '\0';
        url->psz_option = next;
    }

    /* Authority */
    if (strncmp(cur, "//", 2) == 0)
    {
        cur += 2;

        /* Path */
        next = strchr(cur, '/');
        if (next != NULL)
        {
            *next = '\0'; /* temporary nul, reset to slash later */
            url->psz_path = next;
        }
        /*else
            url->psz_path = "/";*/
        printf("path: %s\n", cur);

        /* User name */
        next = strrchr(cur, '@');
        if (next != NULL)
        {
            *(next++) = '\0';
            url->psz_username = cur;
            cur = next;
            printf("username: %s\n", cur);

            /* Password (obsolete) */
            next = strchr(url->psz_username, ':');
            printf("password: %s\n", next);
            if (next != NULL)
            {
                *(next++) = '\0';
                url->psz_password = next;
                //vlc_uri_decode(url->psz_password);
            }
            //vlc_uri_decode(url->psz_username);
        }

        /* Host name */
        if (*cur == '[' && (next = strrchr(cur, ']')) != NULL)
        {   /* Try IPv6 numeral within brackets */
            *(next++) = '\0';
            url->psz_host = _strdup(cur + 1);

            if (*next == ':')
                next++;
            else
                next = NULL;
        }
        else
        {
            next = strchr(cur, ':');
            if (next != NULL)
                *(next++) = '\0';

            //const char* host = vlc_uri_decode(cur);
            //url->psz_host = (host != NULL) ? vlc_idna_to_ascii(host) : NULL;
        }
        printf("host: %s\n", cur);

        //if (url->psz_host == NULL)
        //    ret = -1;
        //else
        //    if (!vlc_uri_host_validate(url->psz_host))
        //    {
        //        free(url->psz_host);
        //        url->psz_host = NULL;
        //        errno = EINVAL;
        //        ret = -1;
        //    }

        /* Port number */
        if (next != NULL && *next)
        {
            char* end;
            unsigned long port = strtoul(next, &end, 10);

            if (strchr("0123456789", *next) == NULL || *end || port > UINT_MAX)
            {
                errno = EINVAL;
                ret = -1;
            }

            url->i_port = port;
        }

        if (url->psz_path != NULL)
            *url->psz_path = '/'; /* restore leading slash */
    }
    else
    {
        url->psz_path = cur;
    }

    return ret;
}

void smb2_parse_url(const char* url)
{
    struct smb2_url* u;
    char* ptr, * tmp, str[1024], host[1024];
    char* args;

    if (strncmp(url, "smb://", 6)) {
        printf("URL does not start with 'smb://'");
        return;
    }
    if (strlen(url + 6) >= 1024) {
        printf("URL is too long");
        return;
    }
    strncpy_s(str, url + 6, 1024);

    args = strchr(str, '?');
    if (args) {
        *(args++) = '\0';
        // do nothing.
    }

    ptr = str;
    char* host_end = strchr(str, '/');
    int len_host = host_end - ptr;
    strncpy_s(host, ptr, len_host);

    printf("ptr: %s\n", ptr);
    printf("host: %s\n", host);

    char* shared_folder = strchr(ptr, '/');
    if (!shared_folder) {
        printf("Wrong URL format");
        return;
    }
    int len_shared_folder = strlen(shared_folder);

    /* domain */
    if ((tmp = strchr(ptr, ';')) != NULL && strlen(tmp) > len_shared_folder) {
        *(tmp++) = '\0';
        printf("domain: %s\n", _strdup(ptr));
        ptr = tmp;
    }
    /* user */
    if ((tmp = strrchr(host, '@')) != NULL && strlen(tmp) > len_shared_folder) {
        *(tmp++) = '\0';
        printf("user: %s\n", _strdup(host));
        ptr = tmp;
    }
    /* server */
    if ((tmp = strchr(host, '/')) != NULL) {
        *(tmp++) = '\0';
        printf("server: %s\n", _strdup(host));
        ptr = tmp;
    }

    printf("server: %s\n", _strdup(ptr));

    /* Do we just have a share or do we have both a share and an object */
    tmp = strchr(ptr, '/');

    /* We only have a share */
    if (tmp == NULL) {
        printf("share: %s\n", _strdup(ptr));
        return;
    }
}

/* RFC3987 ¡ì3.1 */
static char* vlc_iri2uri(const char* iri)
{
    size_t a = 0, u = 0;

    for (size_t i = 0; iri[i] != '\0'; i++)
    {
        unsigned char c = iri[i];

        if (c < 128)
            a++;
        else
            u++;
    }

    if (unlikely((a + u) > (SIZE_MAX / 4)))
    {
        errno = ENOMEM;
        return NULL;
    }

    char* uri = (char*)malloc(a + 3 * u + 1), * p;
    if (unlikely(uri == NULL))
        return NULL;

    for (p = uri; *iri != '\0'; iri++)
    {
        unsigned char c = *iri;

        if (c < 128)
            *(p++) = c;
        else
        {
            *(p++) = '%';
            *(p++) = urihex[c >> 4];
            *(p++) = urihex[c & 0xf];
        }
    }

    *p = '\0';
    return uri;
}

