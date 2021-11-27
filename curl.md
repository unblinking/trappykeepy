# API use examples using `curl`  

## Users  

### Create a user  

```bash
curl --location --request POST 'https://localhost:7294/v1/user' \
--header 'Authorization: Bearer eyJUeXAiOiJKV1QiLCJBbGciOiJIUzI1NiJ9.eyJpZCI6IkgrTHNDaGp3d0FHSENPNWMvNDdBdzd5TGQ3Ujl4eElSVFhYUTF1N3A0dzY4RWRoMmZUYzZMOXIrVFNRU0x1WHZmaTYzM2hOK1EwTHJHU3ZucWZueStRPT0iLCJ0eXBlIjoiQ3hjVnFnMW1kRS9iNmZZeGlsQmVMMFd0ZjRYOFBJeGRheGptNGlVVzdRVT0iLCJpc3N1ZWQiOiJaRjVaTXpGRFpRTGJDaWNMajB4L3JGeVIvWlpuMWFLSFByTS9rOCtWZTRHWVNENldpNEc3R3RUMmt0NVczeVpoIiwiZXhwIjoxNjM3OTkxMjQzLjAzMzU0OTN9.fQE977+9AkwC77+977+977+9CO+/vUBT77+9Ajd8PO+/vVZm77+977+977+977+9Mkfvv73Sku+/vQ==' \
--header 'Content-Type: application/json' \
--data-raw '{
    "Name": "foo",
    "Password": "passwordfoo",
    "Email": "foo@example.com"
}'
```

```json
{
    "status": "success",
    "data": {
        "id": "caa1e262-eb22-4ed1-8cb8-7a557f1e9d24"
    }
}
```

### Read all users  

```bash
curl --location --request GET 'https://localhost:7294/v1/user' \
--header 'Authorization: Bearer eyJUeXAiOiJKV1QiLCJBbGciOiJIUzI1NiJ9.eyJpZCI6IkgrTHNDaGp3d0FHSENPNWMvNDdBdzd5TGQ3Ujl4eElSVFhYUTF1N3A0dzY4RWRoMmZUYzZMOXIrVFNRU0x1WHZmaTYzM2hOK1EwTHJHU3ZucWZueStRPT0iLCJ0eXBlIjoiQ3hjVnFnMW1kRS9iNmZZeGlsQmVMMFd0ZjRYOFBJeGRheGptNGlVVzdRVT0iLCJpc3N1ZWQiOiJaRjVaTXpGRFpRTGJDaWNMajB4L3JGeVIvWlpuMWFLSFByTS9rOCtWZTRHWVNENldpNEc3R3RUMmt0NVczeVpoIiwiZXhwIjoxNjM3OTkxMjQzLjAzMzU0OTN9.fQE977+9AkwC77+977+977+9CO+/vUBT77+9Ajd8PO+/vVZm77+977+977+977+9Mkfvv73Sku+/vQ=='
```

```json
{
    "status": "success",
    "data": [
        {
            "id": "caa1e262-eb22-4ed1-8cb8-7a557f1e9d24",
            "name": "foo",
            "email": "foo@example.com",
            "dateCreated": "2021-11-27T06:05:39"
        }
    ]
}
```

### Read one user  

```bash
curl --location --request GET 'https://localhost:7294/v1/user/caa1e262-eb22-4ed1-8cb8-7a557f1e9d24' \
--header 'Authorization: Bearer eyJUeXAiOiJKV1QiLCJBbGciOiJIUzI1NiJ9.eyJpZCI6IkgrTHNDaGp3d0FHSENPNWMvNDdBdzd5TGQ3Ujl4eElSVFhYUTF1N3A0dzY4RWRoMmZUYzZMOXIrVFNRU0x1WHZmaTYzM2hOK1EwTHJHU3ZucWZueStRPT0iLCJ0eXBlIjoiQ3hjVnFnMW1kRS9iNmZZeGlsQmVMMFd0ZjRYOFBJeGRheGptNGlVVzdRVT0iLCJpc3N1ZWQiOiJaRjVaTXpGRFpRTGJDaWNMajB4L3JGeVIvWlpuMWFLSFByTS9rOCtWZTRHWVNENldpNEc3R3RUMmt0NVczeVpoIiwiZXhwIjoxNjM3OTkxMjQzLjAzMzU0OTN9.fQE977+9AkwC77+977+977+9CO+/vUBT77+9Ajd8PO+/vVZm77+977+977+977+9Mkfvv73Sku+/vQ=='
```

```json
{
    "status": "success",
    "data": {
        "id": "caa1e262-eb22-4ed1-8cb8-7a557f1e9d24",
        "name": "foo",
        "email": "foo@example.com",
        "dateCreated": "2021-11-27T06:05:39"
    }
}
```

### Update one user  

```bash
curl --location --request PUT 'https://localhost:7294/v1/user' \
--header 'Authorization: Bearer eyJUeXAiOiJKV1QiLCJBbGciOiJIUzI1NiJ9.eyJpZCI6IkgrTHNDaGp3d0FHSENPNWMvNDdBdzd5TGQ3Ujl4eElSVFhYUTF1N3A0dzY4RWRoMmZUYzZMOXIrVFNRU0x1WHZmaTYzM2hOK1EwTHJHU3ZucWZueStRPT0iLCJ0eXBlIjoiQ3hjVnFnMW1kRS9iNmZZeGlsQmVMMFd0ZjRYOFBJeGRheGptNGlVVzdRVT0iLCJpc3N1ZWQiOiJaRjVaTXpGRFpRTGJDaWNMajB4L3JGeVIvWlpuMWFLSFByTS9rOCtWZTRHWVNENldpNEc3R3RUMmt0NVczeVpoIiwiZXhwIjoxNjM3OTkxMjQzLjAzMzU0OTN9.fQE977+9AkwC77+977+977+9CO+/vUBT77+9Ajd8PO+/vVZm77+977+977+977+9Mkfvv73Sku+/vQ==' \
--header 'Content-Type: application/json' \
--data-raw '{
    "Id": "caa1e262-eb22-4ed1-8cb8-7a557f1e9d24",
    "Name": "bar",
    "Email": "bar@example.com"
}'
```

```json
{
    "status": "success",
    "data": "User updated."
}
```

### Update one user password  

```bash
curl --location --request PUT 'https://localhost:7294/v1/user/password' \
--header 'Authorization: Bearer eyJUeXAiOiJKV1QiLCJBbGciOiJIUzI1NiJ9.eyJpZCI6IkgrTHNDaGp3d0FHSENPNWMvNDdBdzd5TGQ3Ujl4eElSVFhYUTF1N3A0dzY4RWRoMmZUYzZMOXIrVFNRU0x1WHZmaTYzM2hOK1EwTHJHU3ZucWZueStRPT0iLCJ0eXBlIjoiQ3hjVnFnMW1kRS9iNmZZeGlsQmVMMFd0ZjRYOFBJeGRheGptNGlVVzdRVT0iLCJpc3N1ZWQiOiJaRjVaTXpGRFpRTGJDaWNMajB4L3JGeVIvWlpuMWFLSFByTS9rOCtWZTRHWVNENldpNEc3R3RUMmt0NVczeVpoIiwiZXhwIjoxNjM3OTkxMjQzLjAzMzU0OTN9.fQE977+9AkwC77+977+977+9CO+/vUBT77+9Ajd8PO+/vVZm77+977+977+977+9Mkfvv73Sku+/vQ==' \
--header 'Content-Type: application/json' \
--data-raw '{
    "Id": "caa1e262-eb22-4ed1-8cb8-7a557f1e9d24",
    "password": "passwordbar"
}'
```

```json
{
    "status": "success",
    "data": "User password updated."
}
```

### Delete one user  

```bash
curl --location --request DELETE 'https://localhost:7294/v1/user/caa1e262-eb22-4ed1-8cb8-7a557f1e9d24' \
--header 'Authorization: Bearer eyJUeXAiOiJKV1QiLCJBbGciOiJIUzI1NiJ9.eyJpZCI6IkgrTHNDaGp3d0FHSENPNWMvNDdBdzd5TGQ3Ujl4eElSVFhYUTF1N3A0dzY4RWRoMmZUYzZMOXIrVFNRU0x1WHZmaTYzM2hOK1EwTHJHU3ZucWZueStRPT0iLCJ0eXBlIjoiQ3hjVnFnMW1kRS9iNmZZeGlsQmVMMFd0ZjRYOFBJeGRheGptNGlVVzdRVT0iLCJpc3N1ZWQiOiJaRjVaTXpGRFpRTGJDaWNMajB4L3JGeVIvWlpuMWFLSFByTS9rOCtWZTRHWVNENldpNEc3R3RUMmt0NVczeVpoIiwiZXhwIjoxNjM3OTkxMjQzLjAzMzU0OTN9.fQE977+9AkwC77+977+977+9CO+/vUBT77+9Ajd8PO+/vVZm77+977+977+977+9Mkfvv73Sku+/vQ=='
```

```json
{
    "status": "success",
    "data": "User deleted."
}
```

### Create one user session  

```bash
curl --location --request POST 'https://localhost:7294/v1/user/session' \
--header 'Content-Type: application/json' \
--data-raw '{
    "email": "bar@example.com",
    "password": "passwordbar"
}'
```

```json
{
    "status": "success",
    "data": "eyJUeXAiOiJKV1QiLCJBbGciOiJIUzI1NiJ9.eyJpZCI6IkUvS2pWeUpnRGIreHYybzNjTmhQMHRQajhwZWFHbzlHVllmNVkvTDQvTTk4M1RNN0t5ajdlVjNyUjA4dVJXUUJkekZ6aDNlVmZhblR3TnJyM1dSaHh3PT0iLCJ0eXBlIjoiRU05NnZpaHl2K3p6ZURCajFONU5UTkFNcjZGZlJuS243dGdxL3NXMU02bz0iLCJpc3N1ZWQiOiJHcTU4MlNFV0pmUDVUcHpuVHdUVm9WeUl6R0RGOTIyOCtTUGUvdlUzaUhjTVRub2xxc0NHR2dWWW1TckpkNmNGIiwiZXhwIjoxNjM3OTk1NDg5LjcwMDkxMzd9.Ve+/ve+/ve+/vUJ0fdWFzrPvv73vv70c77+9Hjd6D2RT77+9GD1R77+977+9KWIt77+9"
}
```

## Documents (aka Keepers)  

### Create one document  

```bash
curl --location --request POST 'https://localhost:7294/v1/keeper' \
--header 'Authorization: Bearer eyJUeXAiOiJKV1QiLCJBbGciOiJIUzI1NiJ9.eyJpZCI6IkUvS2pWeUpnRGIreHYybzNjTmhQMHRQajhwZWFHbzlHVllmNVkvTDQvTTk4M1RNN0t5ajdlVjNyUjA4dVJXUUJkekZ6aDNlVmZhblR3TnJyM1dSaHh3PT0iLCJ0eXBlIjoiRU05NnZpaHl2K3p6ZURCajFONU5UTkFNcjZGZlJuS243dGdxL3NXMU02bz0iLCJpc3N1ZWQiOiJHcTU4MlNFV0pmUDVUcHpuVHdUVm9WeUl6R0RGOTIyOCtTUGUvdlUzaUhjTVRub2xxc0NHR2dWWW1TckpkNmNGIiwiZXhwIjoxNjM3OTk1NDg5LjcwMDkxMzd9.Ve+/ve+/ve+/vUJ0fdWFzrPvv73vv70c77+9Hjd6D2RT77+9GD1R77+977+9KWIt77+9' \
--form 'file=@dummy.pdf'
```

```json
{
    "status": "success",
    "data": {
        "id": "3ae5d856-ba2a-45d5-923f-af87016f28a4"
    }
}
```

### Get one document  

```bash
curl --location --request GET 'https://localhost:7294/v1/keeper/3ae5d856-ba2a-45d5-923f-af87016f28a4' \
--header 'Authorization: Bearer eyJUeXAiOiJKV1QiLCJBbGciOiJIUzI1NiJ9.eyJpZCI6IkUvS2pWeUpnRGIreHYybzNjTmhQMHRQajhwZWFHbzlHVllmNVkvTDQvTTk4M1RNN0t5ajdlVjNyUjA4dVJXUUJkekZ6aDNlVmZhblR3TnJyM1dSaHh3PT0iLCJ0eXBlIjoiRU05NnZpaHl2K3p6ZURCajFONU5UTkFNcjZGZlJuS243dGdxL3NXMU02bz0iLCJpc3N1ZWQiOiJHcTU4MlNFV0pmUDVUcHpuVHdUVm9WeUl6R0RGOTIyOCtTUGUvdlUzaUhjTVRub2xxc0NHR2dWWW1TckpkNmNGIiwiZXhwIjoxNjM3OTk1NDg5LjcwMDkxMzd9.Ve+/ve+/ve+/vUJ0fdWFzrPvv73vv70c77+9Hjd6D2RT77+9GD1R77+977+9KWIt77+9' \
--output dummy.pdf
```

```bash
  % Total    % Received % Xferd  Average Speed   Time    Time     Time  Current
                                 Dload  Upload   Total   Spent    Left  Speed
100 13264  100 13264    0     0  47202      0 --:--:-- --:--:-- --:--:-- 47035
```

