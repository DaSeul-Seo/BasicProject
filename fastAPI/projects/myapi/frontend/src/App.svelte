<script>
  import Router from 'svelte-spa-router'
  import Home from './routes/Home.svelte'
  import Detail from './routes/Detail.svelte'
  import QuestionCreate from './routes/QuestionCreate.svelte'
  import Navigation from './components/Navigation.svelte'
  import UserCreate from "./routes/UserCreate.svelte"
  import UserLogin from "./routes/UserLogin.svelte"
  import QuestionModify from "./routes/QuestionModify.svelte"
  import AnswerModify from "./routes/AnswerModify.svelte"
  import Category from './components/Category.svelte'
  //import QuestionDetail from "./routes/QuestionDetail.svelte"


  const routes = {
    '/': Home,
    '/detail/:question_id': Detail, // detail/ 뒤에 가변적이기 때문에
    '/question-create': QuestionCreate,
    '/user-create': UserCreate,
    '/user-login': UserLogin,
    '/question-modify/:question_id': QuestionModify,
    '/answer-modify/:answer_id': AnswerModify,
  }
</script>

<Navigation />

<!-- 전체 레이아웃 -->
<div class="container my-4">
    <div class="row">
      <!-- 카테고리 -->
       <div class="col-3">
          <Category />
       </div>
       <div class="col-9">
          <Router {routes} />
       </div>
    </div>
</div>

