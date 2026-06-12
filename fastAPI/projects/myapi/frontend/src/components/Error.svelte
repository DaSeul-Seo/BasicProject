<!-- alert창이 아닌 컴포넌트로 표시 -->
<script>
    // 전달받은 오류
    export let error
</script>

{#if typeof error.detail == 'string'}
    <div class="alert alert-danger" role="alert">
        <div>
            {error.detail}
        </div>
    </div>
{:else if typeof error.detail == 'object' && error.detail.length > 0}
    <div class="alert alert-danger" role="alert">
        {#each error.detail as err, i}
        <div>
            <strong>{err.loc[1]}</strong> : {err.msg}
        </div>
        {/each}
    </div>
{/if}

<!-- {#if typeof error.detail == 'string'}
    <ul>
        <li>{error.detail}</li>
    </ul>
{:else if typeof error.detail == 'object' && error.detail.length > 0}
    <ul>
        {#each error.detail as err, i}
        <li>
            <strong>{err.loc[1]}</strong> : {err.msg}
        </li>
        {/each}
    </ul>
{/if} -->